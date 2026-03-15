using System;

public class ItemSystem
{
    private Random random = new Random();

    private string selectedItemId;
    private ItemTargetType selectedTargetType;

    // 아이템 생성 및 제공
    public Item GenerateItem()
    {
        int roll = random.Next(100);

        if (roll < 30)
            return new Item(ItemType.BackDo);

        if (roll < 60)
            return new Item(ItemType.StealCoin);

        if (roll < 90)
            return new Item(ItemType.DoubleMove);

        return new Item(ItemType.MoveOtherToken);
    }
}