using System.Collections.Generic;

// 아이템 종류
public static class ItemEffects
{
    public static readonly Dictionary<ItemType, IItemEffect> Effects =
        new Dictionary<ItemType, IItemEffect>
    {
        { ItemType.BackDo, new BackDoItemEffect() },
        { ItemType.StealCoin, new StealCoinItemEffect() },
        { ItemType.DoubleMove, new DoubleMoveItemEffect() },
        { ItemType.MoveOtherToken, new MoveOtherTokenItemEffect() }
    };
}