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

    public RollResult Roll(string playerId)
    {
        if(currentState != GameState.WaitingForThrow)
        {
            return new RollResult
            {
                IsValid = false,
                Error = RollError.InvalidGameState
            };
        }

        // 테스트 위해 주석 처리
        //if (players[currentTurnIndex].PlayerId != playerId)
        //{
        //    return new RollResult
        //    {
        //        IsValid = false,
        //        Error = RollError.NotYourTurn
        //    };
        //}

        var result = yutSystem.Roll();
        currentStep = (int) result;

        if(currentStep <= 0 && currentStep >= 6)
        {
            return new RollResult
            {
                IsValid = false,
                Error = RollError.InvalidStep
            };
        }

        //이후 모, 윷이 나올 경우 결과 저장 및 따로 분기 필요
        currentState = GameState.WaitingForSelect;

        return new RollResult
        {
            IsValid = true,
            ResultStep = currentStep,
            Error = RollError.None
        };
    }

    public MoveResult Move(string tokenId)
    {
        if (currentState != GameState.WaitingForSelect)
        {
            return new MoveResult
            {
                IsValid = false,
                Error = MoveError.InvalidGameState
            };
        }

        Token token = tokens.Find(t => t.TokenId == tokenId);

        if (token == null)
            return new MoveResult
            {
                IsValid = false,
                Error = MoveError.InvalidToken
            };

        if (token.PlayerId != players[currentTurnIndex].PlayerId)
            return new MoveResult
            {
                IsValid = false,
                Error = MoveError.NotYourToken
            };

        Tile destination = board.MoveToken(token, currentStep);

        bool captured = ruleEngine.ResolveCapture(token, destination, players);

        if (!captured)
        {
            currentTurnIndex = (currentTurnIndex + 1) % players.Count;
        }

        currentState = GameState.WaitingForThrow;

        return new MoveResult
            {
                IsValid = true,
                TokenId = token.TokenId,
                NewIndex = token.CurrentTileIndex,
                CurrentTurnPlayerId = players[currentTurnIndex].PlayerId,
                Captured = captured,
                Error = MoveError.None
            };
    }


}
