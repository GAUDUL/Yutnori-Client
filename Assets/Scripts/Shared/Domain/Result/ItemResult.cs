public enum ItemError
{
    NotYourTurn,
    InvalidItem,
    InvalidTarget,
    InvalidTiming
}

public class ItemResult
{
    public bool IsSuccess { get; }
    public ItemError? Error { get; }

    public string PlayerId { get; }
    public ItemType? ItemType { get; }
    public MoveResult MoveResult { get; }

    private ItemResult(bool success, ItemError? error, string playerId, ItemType? itemType, MoveResult moveResult = null)
    {
        IsSuccess = success;
        Error = error;
        PlayerId = playerId;
        ItemType = itemType;
        MoveResult = moveResult;
    }

    public static ItemResult Success(string playerId, ItemType itemType, MoveResult moveResult = null)
    {
        return new ItemResult(true, null, playerId, itemType, moveResult);
    }

    public static ItemResult Fail(ItemError error)
    {
        return new ItemResult(false, error, null, null);
    }
}