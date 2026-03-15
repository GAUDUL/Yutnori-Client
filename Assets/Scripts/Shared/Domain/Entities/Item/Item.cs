using System;

public class Item
{
    public string Id { get; }
    public string Name { get; }
    public ItemType Type { get; }

    public Item(ItemType type)
    {
        Id = Guid.NewGuid().ToString();
        Type = type;
        Name = GetItemName(type);
    }

    private string GetItemName(ItemType type)
    {
        switch (type)
        {
            case ItemType.BackDo:
                return "빽도";
            case ItemType.StealCoin:
                return "코인 뺏기";
            case ItemType.DoubleMove:
                return "x2 이동";
            case ItemType.MoveOtherToken:
                return "말 이동시키기";
            default:
                return "Unknown";
        }
    }
}