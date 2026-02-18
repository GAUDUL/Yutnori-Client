public enum MoveError
{
    None,
    NotYourTurn,
    NotYourToken,
    InvalidToken,
    InvalidStep,
    InvalidGameState
}

public class MoveResult
{
    public MoveError Error { get; }
    public string TokenId { get; }
    public int NewIndex { get; }
    public string CurrentTurnPlayerId { get; }
    public bool Captured { get; }

    public bool IsSuccess => Error == MoveError.None;

    private MoveResult(
        MoveError error,
        string tokenId = null,
        int newIndex = 0,
        string currentTurnPlayerId = null,
        bool captured = false)
    {
        Error = error;
        TokenId = tokenId;
        NewIndex = newIndex;
        CurrentTurnPlayerId = currentTurnPlayerId;
        Captured = captured;
    }

    public static MoveResult Fail(MoveError error)
    {
        return new MoveResult(error);
    }

    public static MoveResult Success(
        string tokenId,
        int newIndex,
        string currentTurnPlayerId,
        bool captured)
    {
        return new MoveResult(
            MoveError.None,
            tokenId,
            newIndex,
            currentTurnPlayerId,
            captured);
    }
}
