public class MergeResult
{
    public bool IsSuccess { get; }
    public string CurrentTurnPlayerId { get; }
    public bool IsRoundEnd { get; }

    private MergeResult(bool isSuccess, string currentTurnPlayerId, bool isRoundEnd)
    {
        IsSuccess = isSuccess;
        CurrentTurnPlayerId = currentTurnPlayerId;
        IsRoundEnd = isRoundEnd;
    }

    public static MergeResult Success(string currentTurnPlayerId, bool isRoundEnd)
    {
        return new MergeResult(true, currentTurnPlayerId, isRoundEnd);
    }

    public static MergeResult Fail()
    {
        return new MergeResult(false, null, false);
    }
}