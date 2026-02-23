using System.Collections.Generic;

public class MoveValidator
{
    private List<Token> tokens;

    public MoveValidator(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public (MoveResult Validation, Token Token) Validate(string tokenId, string currentPlayerId, bool isMovePhase)
    {
        if (!isMovePhase)
            return (MoveResult.Fail(MoveError.InvalidGameState), null);

        Token token = tokens.Find(t => t.TokenId == tokenId);
        if (token == null)
            return (MoveResult.Fail(MoveError.InvalidToken), null);

        if (token.PlayerId != currentPlayerId)
            return (MoveResult.Fail(MoveError.NotYourToken), null);

        return (null, token);
    }
}