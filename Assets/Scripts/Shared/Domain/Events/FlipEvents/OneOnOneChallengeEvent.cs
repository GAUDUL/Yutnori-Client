using System;
using System.Collections.Generic;

// 일대일대결
public class OneOnOneChallengeEvent : IFlipEvent
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