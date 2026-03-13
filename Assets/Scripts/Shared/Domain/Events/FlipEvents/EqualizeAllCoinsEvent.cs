
using System;
using System.Collections.Generic;
using System.Linq;

// 모든 플레이어 코인 균등 배분
public class EqualizeAllCoinsEvent : IFlipEvent
{
    public void Execute(Player currentPlayer, List<Player> players)
    {
        int totalCoins = players.Sum(p => p.Coin);
        // 코인 초기화
        foreach (var p in players)
            p.LoseCoin(p.Coin);

        int perPlayerGain = (int)Math.Round((double)totalCoins / players.Count);

        foreach (var p in players)
            p.AddCoin(perPlayerGain);
    }
}