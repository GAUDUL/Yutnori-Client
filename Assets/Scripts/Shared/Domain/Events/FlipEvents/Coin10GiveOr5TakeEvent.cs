using System;
using System.Collections.Generic;
using System.Linq;


// 코인 10개 주기 or 5개 뺏기 (대상: poorest)
public class Coin10GiveOr5TakeEvent : IFlipEvent
{
    private Random random = new Random();

    public void Execute(Player currentPlayer, List<Player> players)
    {
        int minCoin = players.Min(p => p.Coin);

        // poorest 리스트
        var poorestPlayers = players
            .Where(p => p.Coin == minCoin && p.PlayerId != currentPlayer.PlayerId)
            .ToList();

        if (!poorestPlayers.Any()) return;

        foreach (var poorest in poorestPlayers)
        {
            int subRoll = random.Next(0, 10);

            if (subRoll < 7)
            {
                int actualTake = currentPlayer.LoseCoin(10);
                poorest.AddCoin(actualTake);
            }
            else
            {
                int actualTake = poorest.LoseCoin(5);
                currentPlayer.AddCoin(actualTake);
            }
        }
    }
}
