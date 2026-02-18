public enum RollError
{
    None,
    NotYourTurn,
    InvalidStep,
    InvalidGameState
}

public class RollResult
{
    public RollError Error { get; }
    public int ResultStep { get; }
    public bool ExtraTurn { get; }

    public bool IsSuccess => Error == RollError.None;

    private RollResult(
        RollError error,
        int resultStep = 0,
        bool extraTurn = false)
    {
        Error = error;
        ResultStep = resultStep;
        ExtraTurn = extraTurn;
    }

    public static RollResult Fail(RollError error)
    {
        return new RollResult(error);
    }

    public static RollResult Success(int step, bool extraTurn)
    {
        return new RollResult(
            RollError.None,
            step,
            extraTurn);
    }
}
