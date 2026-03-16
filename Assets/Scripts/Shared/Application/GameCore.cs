using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Rendering;
using static YutSystem;

public class GameCore
{
    private enum GameState
    {
        WaitingForThrow,   // Roll 대기
        WaitingForTokenSelect, // Token 선택
        WaitingForStepSelect,   // Step 선택
        WaitingForMerge, // 업기 대기 상태
    }

    private Board board;
    private RuleEngine ruleEngine;
    private YutSystem yutSystem;
    private MoveSystem moveSystem;
    private TileEffectSystem tileEffectSystem;
    private MapEventSystem mapEventSystem;
    private TurnManager turnManager;

    private MoveValidator moveValidator;
    private RollValidator rollValidator;

    private Dictionary<string, Player> playersById;

    private List<int> pendingSteps = new List<int>(); // 윷 던지기 결과 저장
    private string selectedTokenId; // 선택된 말
    private List<TokenGroup> mergeCandidates; // 업기 후보 그룹
    private Tile mergeTile; //업기 발생 타일

    private GameState currentState;
    bool doubleMoveActive;
    private int? forcedNextRoll = null;
    private MoveResult itemMoveResult;

    public GameCore(int boardSize, Dictionary<string, Player> playersById, List<Token> tokens)
    {
        board = new Board(boardSize, MapData.Default());
        ruleEngine = new RuleEngine();
        yutSystem = new YutSystem();
        turnManager = new TurnManager(new List<string>(playersById.Keys));
        moveValidator = new MoveValidator(board);
        rollValidator = new RollValidator();

        this.playersById = playersById;

        // 각 토큰을 그룹으로 생성
        foreach (var token in tokens)
            board.CreateInitialGroup(token);

        moveSystem = new MoveSystem(board);
        mapEventSystem = new MapEventSystem(turnManager);
        tileEffectSystem = new TileEffectSystem(this, mapEventSystem);

        currentState = GameState.WaitingForThrow;
    }

    public Dictionary<string, Player> PlayersById => playersById;
    public string CurrentTurnPlayerId => turnManager.CurrentPlayerId;
    public Player CurrentPlayer => playersById[turnManager.CurrentPlayerId];
    public string CurrentTurnPlaeyrName => CurrentPlayer.DisplayName;

    public bool CanSelectStep => currentState == GameState.WaitingForStepSelect;
    public bool CanUseItem => currentState == GameState.WaitingForThrow;

    public void AddStep(int step)
    {
        pendingSteps.Add(step);
    }
    public void EnableDoubleMove()
    {
        doubleMoveActive = true;
    }

    public IReadOnlyList<int> GetPendingSteps()
    {
        return pendingSteps;
    }

    // 보드 타일 반환
    public Tile[] GetTiles()
    {
        return board.GetTiles();
    }

    // Roll 결과 강제 고정
    public void ForceNextRoll(int step)
    {
        forcedNextRoll = step;
    }

    // 윷 던지기
    public RollResult Roll(string playerId)
    {
        // 현재 턴 플레이어Id가 맞는지 확인
        // 테스트 위해 주석 처리
        //if (CurrentTurnPlayerId != playerId)
        //    return RollResult.Fail(RollError.NotYourTurn);

        var validation = rollValidator.Validate(currentState == GameState.WaitingForThrow, turnManager.RemainingRolls);
        if (!validation.IsSuccess)
            return validation;

        int step = 0;
        YutResult result;

        // 빽도 아이템
        if (forcedNextRoll.HasValue)
        {
            step = forcedNextRoll.Value;
            forcedNextRoll = null;

            result = YutResult.BackDo; // 1회 사용
        }
        else
        {
            result = yutSystem.Roll();
            step = (int)result;
        }


        if (step < -1 || step >= 6)
            return RollResult.Fail(RollError.InvalidStep);

        turnManager.UseRoll();
        pendingSteps.Add(step);

        // 윷 or 모 확인
        if (yutSystem.IsExtraTurn(result))
            turnManager.GrantExtraTurnYut();

        // 던지기 기회 남아있을 경우 Roll 대기 상태
        currentState = (turnManager.RemainingRolls > 0) ? GameState.WaitingForThrow : GameState.WaitingForTokenSelect;

        return RollResult.Success(step, turnManager.RemainingRolls > 0);
    }



    // 이동할 말 선택
    public bool SelectToken(string tokenId)
    {
        if (pendingSteps.Count == 0)
            return false;

        bool isSelectableState =
            currentState == GameState.WaitingForTokenSelect ||
            currentState == GameState.WaitingForStepSelect;

        var (validation, tokenGroup) =
            moveValidator.Validate(tokenId, CurrentTurnPlayerId, isSelectableState);

        if (validation != null)
            return false;

        selectedTokenId = tokenId;
        currentState = GameState.WaitingForStepSelect;

        return true;
    }

    // 말 이동
    public MoveResult Move(int selectedStep)
    {
        if (selectedTokenId == null)
            return MoveResult.Fail(MoveError.InvalidToken);

        // 이동할 토큰 그룹
        var (validation, tokenGroup) = moveValidator.Validate(selectedTokenId, CurrentTurnPlayerId, currentState == GameState.WaitingForStepSelect);
        if (validation != null)
            return validation;

        if (pendingSteps.Count == 0)
            return MoveResult.Fail(MoveError.NoStep);

        if (!pendingSteps.Contains(selectedStep))
            return MoveResult.Fail(MoveError.InvalidStep);

        pendingSteps.Remove(selectedStep);

        // 2배 이동 아이템 사용
        if (doubleMoveActive) 
        {
            selectedStep *= 2;
            doubleMoveActive = false;
        }

        // 말 이동 처리
        var (destination, lapCount) = moveSystem.ExecuteMove(tokenGroup, selectedStep, CurrentPlayer);

        bool isTeleport = false;

        // 연결된 칸 도착 시
        if (destination.ConnectedTileIndex.HasValue)
        {
            int targetIndex = destination.ConnectedTileIndex.Value;

            destination.ConnectedTileIndex = null;
            board.GetTile(targetIndex).ConnectedTileIndex = null;

            destination = board.TeleportTokenGroup(tokenGroup, targetIndex);

            isTeleport = true;
        }

        // 타일 기믹 실행
        var tileEffectResult = tileEffectSystem.Execute(destination, CurrentPlayer);

        // 말 잡기 처리
        bool captured = false;
        if (!isTeleport)
        {
            captured = ruleEngine.ResolveCapture(tokenGroup, destination, playersById);
        }

        // 잡았으면 추가 턴 제공
        if (captured && !turnManager.UsedCaptureExtraTurn)
            turnManager.GrantExtraTurnCapture();

        // 업기 가능 여부
        var groups = ruleEngine.GetGroupCandidates(destination, CurrentTurnPlayerId);
        bool needMerge = groups.Count >= 2;

        bool isRoundEnd = false;
        
        //업기 가능
        if (needMerge)
        {
            mergeCandidates = groups;
            mergeTile = destination;

            currentState = GameState.WaitingForMerge;

            selectedTokenId = null;

            return MoveResult.Success(
                tokenGroup.GroupId,
                tokenGroup.GetTokenIds(),
                tokenGroup.CurrentTileIndex,
                CurrentTurnPlayerId,
                captured,
                false,
                true,
                isTeleport,
                tileEffectResult
            );
        }

        // 턴 확인
        var (roundEnd, restoreResult) = ResolveTurnFlow();
        isRoundEnd = roundEnd;

        if (restoreResult != null)
        {
            if (tileEffectResult == null)
                tileEffectResult = restoreResult;
            else
            {
                var merged = new List<Tile>();
                merged.AddRange(tileEffectResult.ChangedTiles);
                merged.AddRange(restoreResult.ChangedTiles);
                tileEffectResult = new TileEffectResult(merged.ToArray(), tileEffectResult.IsConnected);
            }
        }

        selectedTokenId = null;

        return MoveResult.Success(
            tokenGroup.GroupId,
            tokenGroup.GetTokenIds(), 
            tokenGroup.CurrentTileIndex, 
            CurrentTurnPlayerId, 
            captured, 
            isRoundEnd,
            needMerge,
            isTeleport,
            tileEffectResult
        );
    }

    public MergeResult MergeSelected(bool merge)
    {
        if (currentState != GameState.WaitingForMerge)
            return MergeResult.Fail();

        if (merge && mergeCandidates != null && mergeCandidates.Count >= 2)
        {
            ruleEngine.ResolveMerge(mergeCandidates, mergeTile);
        }

        mergeCandidates = null;
        mergeTile = null;

        var (isRoundEnd, restoreResult) = ResolveTurnFlow();
        return MergeResult.Success(CurrentTurnPlayerId, isRoundEnd);
    }

    // 현재 Turn State 설정 & 턴 확인
    private (bool isRoundEnd, TileEffectResult restoreResult) ResolveTurnFlow()
    {
        if (turnManager.RemainingRolls > 0)
        {
            currentState = GameState.WaitingForThrow;
            return (false, null);
        }

        // 이동 횟수 남아있을 경우 말 선택 대기
        if (pendingSteps.Count > 0)
        {
            currentState = GameState.WaitingForTokenSelect;
            return (false, null);
        }

        bool isRoundEnd = turnManager.EndTurn();

        var restoredTiles = mapEventSystem.TickEvents(board);

        currentState = GameState.WaitingForThrow;

        if (restoredTiles != null)
            return (isRoundEnd, new TileEffectResult(restoredTiles, false));

        return (isRoundEnd, null);
    }

    // 아이템 사용
    public ItemResult UseItem(string playerId, string itemId, string targetPlayerId, string targetTokenId)
    {
        if (playerId != CurrentTurnPlayerId)
            return ItemResult.Fail(ItemError.NotYourTurn);

        var player = playersById[playerId];
        var item = player.GetItemById(itemId);

        if (item == null)
            return ItemResult.Fail(ItemError.InvalidItem);

        if (!ItemEffects.Effects.TryGetValue(item.Type, out var effect))
            return ItemResult.Fail(ItemError.InvalidItem);

        Player targetPlayer = null;
        TokenGroup targetTokenGroup = null;

        if (targetPlayerId != null)
            targetPlayer = playersById[targetPlayerId];

        if(targetTokenId != null)
            targetTokenGroup = board.GetTokenGroup(targetTokenId);

        itemMoveResult = null;

        // 아이템 사용
        effect.Apply(this, player, targetPlayer, targetTokenGroup);

        player.RemoveItem(item);

        return ItemResult.Success(playerId, item.Type, itemMoveResult);
    }

    // 잡기 x Move
    public (Tile tile, int lapCount) MoveTokenWithoutCapture(TokenGroup tokenGroup, int step, Player player)
    {
        var (destination, lapCount) = moveSystem.ExecuteMove(tokenGroup, step, player);

        var tileEffectResult = tileEffectSystem.Execute(destination, player);

        itemMoveResult = MoveResult.Success(
            tokenGroup.GroupId,
            tokenGroup.GetTokenIds(),
            tokenGroup.CurrentTileIndex,
            CurrentTurnPlayerId,
            false,
            false,
            false,
            false,
            tileEffectResult
        );


        return (destination, lapCount);
    }

}