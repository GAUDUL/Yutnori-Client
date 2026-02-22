using System.Collections.Generic;

public class GameCore
{
    private enum GameState
    {
        WaitingForThrow,   // Roll 가능
        WaitingForSelect   // Move 가능
    }

    private Board board;
    private RuleEngine ruleEngine;
    private YutSystem yutSystem;
    private TurnManager turnManager;

    private Dictionary<string, Player> playersById;
    private List<Token> tokens;

    private MoveValidator moveValidator;
    private RollValidator rollValidator;

    private Queue<int> pendingSteps = new Queue<int>(); // 윷 던지기 결과 저장
    private GameState currentState;

    public GameCore(int boardSize, Dictionary<string, Player> playersById, List<Token> tokens)
    {
        board = new Board(boardSize);
        ruleEngine = new RuleEngine();
        yutSystem = new YutSystem();
        turnManager = new TurnManager(new List<string>(playersById.Keys));
        moveValidator = new MoveValidator(tokens);
        rollValidator = new RollValidator();

        this.playersById = playersById;
        this.tokens = tokens;

        foreach (var token in tokens)
            board.PlaceAtStart(token);

        currentState = GameState.WaitingForThrow;
    }

    public string CurrentTurnPlayerId => turnManager.CurrentPlayerId;

    // 윷 던지기
    public RollResult Roll(string playerId)
    {
        var validation = rollValidator.Validate(currentState == GameState.WaitingForThrow, turnManager.RemainingRolls);
        if (!validation.IsSuccess)
            return validation;

        var result = yutSystem.Roll();
        int step = (int)result;

        if (step < -1 || step >= 6)
            return RollResult.Fail(RollError.InvalidStep);

        turnManager.UseRoll();
        pendingSteps.Enqueue(step);

        // 윷 or 모 확인
        if (yutSystem.IsExtraTurn(result))
            turnManager.GrantExtraTurnYut();

        // 던지기 기회 남아있을 경우 Roll 대기 상태
        currentState = (turnManager.RemainingRolls > 0) ? GameState.WaitingForThrow : GameState.WaitingForSelect;

        return RollResult.Success(step, turnManager.RemainingRolls > 0);
    }

    // 말 이동
    public MoveResult Move(string tokenId)
    {
        var (validation, token) = moveValidator.Validate(tokenId, CurrentTurnPlayerId, pendingSteps, currentState == GameState.WaitingForSelect);
        if (validation != null)
            return validation;

        int step = pendingSteps.Dequeue();
        Tile destination = board.MoveToken(token, step);

        bool captured = ruleEngine.ResolveCapture(token, destination, playersById);
        bool isRoundEnd = false;

        // 잡으면 추가 턴
        if (captured && !turnManager.UsedCaptureExtraTurn)
        {
            turnManager.GrantExtraTurnCapture();
            currentState = GameState.WaitingForThrow;
            return MoveResult.Success(token.TokenId, token.CurrentTileIndex, CurrentTurnPlayerId, captured, isRoundEnd);
        }

        // 이동 횟수 남아있을 경우 이동 대기
        if (pendingSteps.Count > 0)
        {
            currentState = GameState.WaitingForSelect;
        }
        else
        {
            isRoundEnd = turnManager.EndTurn();
            currentState = GameState.WaitingForThrow;
        }

        return MoveResult.Success(token.TokenId, token.CurrentTileIndex, CurrentTurnPlayerId, captured, isRoundEnd);
    }
}