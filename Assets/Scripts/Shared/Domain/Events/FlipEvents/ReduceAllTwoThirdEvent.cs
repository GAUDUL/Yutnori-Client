using System;
using System.Collections.Generic;

// 모든 플레이어 코인 2/3
public class ReduceAllTwoThirdEvent : IFlipEvent
{
    public void Execute(Player currentPlayer, List<Player> players)
    {
        foreach (var p in players)
        {
            int lost = (int)Math.Ceiling(p.Coin * 1.0 / 3);
            p.LoseCoin(lost);
        }
    }
}