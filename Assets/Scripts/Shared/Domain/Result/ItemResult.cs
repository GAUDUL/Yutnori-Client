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

    private ItemResult(bool success, ItemError? error, string playerId, ItemType? itemType)
    {
        IsSuccess = success;
        Error = error;
        PlayerId = playerId;
        ItemType = itemType;
    }

    public static ItemResult Success(string playerId, ItemType itemType)
    {
        return new ItemResult(true, null, playerId, itemType);
    }

    public static ItemResult Fail(ItemError error)
    {
        return new ItemResult(false, error, null, null);
    }
}