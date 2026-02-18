public enum RollError
{
    None,
    NotYourTurn,
    InvalidStep,
    InvalidGameState
}
public class RollResult
{
    public bool IsValid;
    public RollError Error;

    public int ResultStep;
    public bool ExtraTurn;
}