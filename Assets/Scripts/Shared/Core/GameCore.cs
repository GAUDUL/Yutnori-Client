using System.Collections.Generic;

public class GameCore
{
    private enum GameState
    {
        WaitingForThrow,   // 윷 던지기 대기
        WaitingForSelect   // 말 선택 대기
    }

    private Board board;
    private RuleEngine ruleEngine;
    private YutSystem yutSystem;

    private List<Player> players;
    private List<Token> tokens;

    private GameState currentState;
    private int currentTurnIndex;
    private int currentStep;

    public GameCore(int boardSize, List<Player> players, List<Token> tokens)
    {
        board = new Board(boardSize);
        ruleEngine = new RuleEngine();
        yutSystem = new YutSystem();
        this.players = players;
        this.tokens = tokens;
        currentState = GameState.WaitingForThrow;

        foreach (var token in tokens)
            board.PlaceAtStart(token);
    }

    public string CurrentTurnPlayerId
    {
        get { return players[currentTurnIndex].PlayerId; }
    }

    //윷 던지기
    public RollResult Roll(string playerId)
    {
        var validation = ValidateRoll(playerId);
        if (!validation.IsSuccess)
            return validation;

        var result = yutSystem.Roll();
        currentStep = (int) result;

        if (currentStep <= 0 || currentStep >= 6)
            return RollResult.Fail(RollError.InvalidStep);

        //이후 모, 윷이 나올 경우 결과 저장 및 따로 분기 필요
        currentState = GameState.WaitingForSelect;

        return RollResult.Success(currentStep, extraTurn: false);
    }

    // Roll 검증
    private RollResult ValidateRoll(string playerId)
    {
        if (currentState != GameState.WaitingForThrow)
            return RollResult.Fail(RollError.InvalidGameState);

        // 테스트 위해 주석 처리
        //if (players[currentTurnIndex].PlayerId != playerId)
        //    return RollResult.Invalid(RollError.NotYourTurn);

        return RollResult.Success(0, extraTurn: false);
    }


    // 말 이동
    public MoveResult Move(string tokenId)
    {
        var (validation, token) = ValidateMove(tokenId);
        if (validation != null) return validation;

        Tile destination = board.MoveToken(token, currentStep);
        bool captured = ruleEngine.ResolveCapture(token, destination, players);

        int nextTurnIndex = captured ? currentTurnIndex : (currentTurnIndex + 1) % players.Count;
        currentTurnIndex = nextTurnIndex;

        currentState = GameState.WaitingForThrow;

        return MoveResult.Success(
            token.TokenId,
            token.CurrentTileIndex,
            players[currentTurnIndex].PlayerId,
            captured
        );
    }

    // Move 검증
    private (MoveResult Validation, Token Token) ValidateMove(string tokenId)
    {
        if (currentState != GameState.WaitingForSelect)
            return (MoveResult.Fail(MoveError.InvalidGameState), null);

        Token token = tokens.Find(t => t.TokenId == tokenId);
        if (token == null)
            return (MoveResult.Fail(MoveError.InvalidToken), null);

        if (token.PlayerId != players[currentTurnIndex].PlayerId)
            return (MoveResult.Fail(MoveError.NotYourToken), null);

        return (null, token);
    }

}
