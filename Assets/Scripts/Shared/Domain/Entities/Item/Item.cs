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
                return "BackDo";
            case ItemType.StealCoin:
                return "StealCoin";
            case ItemType.DoubleMove:
                return "DoubleMove";
            case ItemType.MoveOtherToken:
                return "MoveOtherToken";
            default:
                return "Unknown";
        }
    }
}