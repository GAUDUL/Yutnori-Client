using System.Collections.Generic;

public class MoveValidator
{
    private Board board;

    public MoveValidator(Board board)
    {
        this.board = board;
    }

    public (MoveResult Validation, TokenGroup TokenGroup) Validate(string tokenId, string currentPlayerId, bool isMovePhase)
    {
        if (!isMovePhase)
            return (MoveResult.Fail(MoveError.InvalidGameState), null);

        TokenGroup foundGroup = null;

        // 각 그룹 안의 말 탐색
        foreach (var group in board.GetAllGroups())
        {
            var token = group.Tokens.Find(t => t.TokenId == tokenId);
            if (token != null)
            {
                foundGroup = group;
                break;
            }
        }

        if (foundGroup == null)
            return (MoveResult.Fail(MoveError.InvalidToken), null);

        if (foundGroup.PlayerId != currentPlayerId)
            return (MoveResult.Fail(MoveError.NotYourToken), null);

        return (null, foundGroup);
    }
}