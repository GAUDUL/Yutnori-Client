using System.Collections.Generic;

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
    private TurnManager turnManager;

    private Dictionary<string, Player> playersById;
    private List<Token> tokens;

    private MoveValidator moveValidator;
    private RollValidator rollValidator;

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
        this.tokens = tokens;

        // 각 토큰을 그룹으로 생성
        foreach (var token in tokens)
            board.CreateInitialGroup(token);

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
        var (destination, lapCount) = board.MoveTokenGroup(tokenGroup, selectedStep);

        // 한 바퀴 돌기 완료
        if (lapCount > 0)
        {
            int rewardPerLap = tokenGroup.IsGrouped ? 60 : 40;
            CurrentPlayer.AddCoin(rewardPerLap * lapCount);
        }

        // 타일 기믹 실행
        ExecuteTileEffect(destination, CurrentPlayer);

        // 이동한 토큰 그룹에 속한 토큰들 List
        List<string> movedTokenIds = new List<string>();
        foreach (var t in tokenGroup.Tokens)
        {
            movedTokenIds.Add(t.TokenId);
        }

        bool captured = ruleEngine.ResolveCapture(tokenGroup, destination, playersById);
        // 잡았으면 추가 턴 제공
        bool grantExtraTurnByCapture = captured && !turnManager.UsedCaptureExtraTurn;

        if (grantExtraTurnByCapture)
        {
            turnManager.GrantExtraTurnCapture();
        }

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
                movedTokenIds,
                tokenGroup.CurrentTileIndex,
                CurrentTurnPlayerId,
                captured,
                isRoundEnd,
                needMerge
            );
        }

        isRoundEnd = ResolveTurnFlow();

        selectedTokenId = null;

        return MoveResult.Success(
            tokenGroup.GroupId, 
            movedTokenIds, 
            tokenGroup.CurrentTileIndex, 
            CurrentTurnPlayerId, 
            captured, 
            isRoundEnd,
            needMerge
        );
    }

    // 타일 기믹 실행
    private void ExecuteTileEffect(Tile tile, Player player)
    {
        var tileEffects = TileEffects.Default();
        if (tileEffects.TryGetValue(tile.Type, out var effect))
        {
            effect.Execute(player, tile);
        }
    }

    public void MergeSelected(bool merge)
    {
        if (currentState != GameState.WaitingForMerge)
            return;

        if (merge && mergeCandidates != null && mergeCandidates.Count >= 2)
        {
            var baseGroup = mergeCandidates[0];
            var targetGroup = mergeCandidates[1];

            mergeTile.tokenGroups.Remove(targetGroup);

            baseGroup.Merge(targetGroup);
        }

        mergeCandidates = null;
        mergeTile = null;

        ResolveTurnFlow();

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