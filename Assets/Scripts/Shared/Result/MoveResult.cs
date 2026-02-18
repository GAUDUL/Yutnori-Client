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
    public bool IsValid;
    public MoveError Error;

    public string TokenId;
    public int NewIndex;
    public string CurrentTurnPlayerId;
    public bool Captured;
}