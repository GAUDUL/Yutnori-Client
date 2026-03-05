using System.Collections.Generic;
using UnityEngine.Rendering;

public class GameCore
{
    private enum GameState
    {
        WaitingForThrow,   // Roll 대기
        WaitingForTokenSelect, // Token 선택
        WaitingForStepSelect,   // Step 선택
        WaitingForMerge // 업기 대기 상태
    }

    private Board board;
    private RuleEngine ruleEngine;
    private YutSystem yutSystem;
    private MoveSystem moveSystem;
    private TileEffectSystem tileEffectSystem;
    private TurnManager turnManager;

    private MoveValidator moveValidator;
    private RollValidator rollValidator;

    private Dictionary<string, Player> playersById;

    private List<int> pendingSteps = new List<int>(); // 윷 던지기 결과 저장
    private string selectedTokenId; // 선택된 말
    private List<TokenGroup> mergeCandidates; // 업기 후보 그룹
    private Tile mergeTile; //업기 발생 타일

    private GameState currentState;

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
        tileEffectSystem = new TileEffectSystem();

        currentState = GameState.WaitingForThrow;
    }

    public string CurrentTurnPlayerId => turnManager.CurrentPlayerId;
    public Player CurrentPlayer => playersById[turnManager.CurrentPlayerId];
    public bool CanSelectStep => currentState == GameState.WaitingForStepSelect;
    public IReadOnlyList<int> GetPendingSteps()
    {
        return pendingSteps;
    }

    // 보드 타일 반환
    public Tile[] GetTiles()
    {
        return board.GetTiles();
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

        var result = yutSystem.Roll();
        int step = (int)result;

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

        var (validation, token) =
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

        // 말 이동 처리
        var (destination, lapCount) = moveSystem.ExecuteMove(tokenGroup, selectedStep, CurrentPlayer);

        // 타일 기믹 실행
        tileEffectSystem.Execute(destination, CurrentPlayer);

        // 말 잡기 처리
        bool captured = ruleEngine.ResolveCapture(tokenGroup, destination, playersById);

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
                true
            );
        }

        isRoundEnd = ResolveTurnFlow();

        selectedTokenId = null;

        return MoveResult.Success(
            tokenGroup.GroupId,
            tokenGroup.GetTokenIds(), 
            tokenGroup.CurrentTileIndex, 
            CurrentTurnPlayerId, 
            captured, 
            isRoundEnd,
            needMerge
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

        bool isRoundEnd = ResolveTurnFlow();
        return MergeResult.Success(CurrentTurnPlayerId, isRoundEnd);
    }

    // 현재 Turn State 설정
    private bool ResolveTurnFlow()
    {
        if (turnManager.RemainingRolls > 0)
        {
            currentState = GameState.WaitingForThrow;
            return false;
        }

        // 이동 횟수 남아있을 경우 말 선택 대기
        if (pendingSteps.Count > 0)
        {
            currentState = GameState.WaitingForTokenSelect;
            return false;
        }

        bool isRoundEnd = turnManager.EndTurn();
        currentState = GameState.WaitingForThrow;
        return isRoundEnd;
    }

}