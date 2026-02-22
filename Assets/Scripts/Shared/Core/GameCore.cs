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

    private Dictionary<string, Player> playersById;
    private List<string> playerOrder;
    private List<Token> tokens;

    private GameState currentState;
    private int currentTurnIndex;

    private int turnSequence = 0;
    private int roundSequence = 0;

    private int currentStep;
    private int extraTurnCount; //추가 턴 관리

    private bool usedYutExtraTurnThisTurn; //윷 or 모로 얻은 추가 턴
    private bool usedCaptureExtraTurnThisTurn; // 잡기로 얻은 추가 턴

    public GameCore(int boardSize, Dictionary<string, Player> playersById, List<Token> tokens)
    {
        board = new Board(boardSize);
        ruleEngine = new RuleEngine();
        yutSystem = new YutSystem();

        this.playersById = playersById;
        this.tokens = tokens;
        currentState = GameState.WaitingForThrow;

        playerOrder = new List<string>(playersById.Keys);
        currentTurnIndex = 0;

        foreach (var token in tokens)
            board.PlaceAtStart(token);
    }

    public string CurrentTurnPlayerId
    {
        get { return playerOrder[currentTurnIndex]; }
    }

    //윷 던지기
    public RollResult Roll(string playerId)
    {
        var validation = ValidateRoll(playerId);
        if (!validation.IsSuccess)
            return validation;

        var result = yutSystem.Roll();
        currentStep = (int) result;

        if (currentStep < -1 || currentStep >= 6)
            return RollResult.Fail(RollError.InvalidStep);

        bool isYutOrMo =
            result == YutSystem.YutResult.Yut ||
            result == YutSystem.YutResult.Mo;

        // 결과 윷 or 모 & 윷, 모로 얻은 추가 턴 기록 x 일 경우
        // 추가 턴 제공
        if (isYutOrMo && !usedYutExtraTurnThisTurn)
        {
            extraTurnCount++;
            usedYutExtraTurnThisTurn = true;
        }

        bool hasExtraTurn = extraTurnCount > 0 ? true : false;
        currentState = GameState.WaitingForSelect;

        return RollResult.Success(currentStep, hasExtraTurn);
    }

    // Roll 검증
    private RollResult ValidateRoll(string playerId)
    {
        if (currentState != GameState.WaitingForThrow)
            return RollResult.Fail(RollError.InvalidGameState);

        // 현재 턴 플레이어Id가 맞는지 확인
        // 테스트 위해 주석 처리
        //if (CurrentTurnPlayerId != playerId)
        //    return RollResult.Invalid(RollError.NotYourTurn);

        return RollResult.Success(0, extraTurn: false);
    }


    // 말 이동
    public MoveResult Move(string tokenId)
    {
        var (validation, token) = ValidateMove(tokenId);
        if (validation != null) return validation;

        Tile destination = board.MoveToken(token, currentStep);

        bool captured = ruleEngine.ResolveCapture(token, destination, playersById);


        if (captured && !usedCaptureExtraTurnThisTurn)
        {
            extraTurnCount++;
            usedCaptureExtraTurnThisTurn = true;
        }

        bool isRoundEnd = EndTurn();

        currentState = GameState.WaitingForThrow;

        return MoveResult.Success(
            token.TokenId,
            token.CurrentTileIndex,
            CurrentTurnPlayerId,
            captured,
            isRoundEnd
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

        if (token.PlayerId != CurrentTurnPlayerId)
            return (MoveResult.Fail(MoveError.NotYourToken), null);

        return (null, token);
    }

    private bool EndTurn()
    {
        if (extraTurnCount > 0)
        {
            extraTurnCount--;
            return false;
        }

        int nextTurnIndex = (currentTurnIndex + 1) % playerOrder.Count;
        bool isRoundEnd = nextTurnIndex == 0;

        currentTurnIndex = nextTurnIndex;

        usedYutExtraTurnThisTurn = false;
        usedCaptureExtraTurnThisTurn = false;

        return isRoundEnd;
    }

}
