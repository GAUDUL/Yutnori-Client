using System.Collections.Generic;
using System.Linq;

// 제일 많은 코인 소지자에게서 코인 뺏기
public class TakeFromRichestEvent : IFlipEvent
{
    public void Execute(Player currentPlayer, List<Player> players)
    {
        int maxCoin = players
              .Max(p => p.Coin);

        var richestPlayers = players
                             .Where(p => p.Coin == maxCoin)
                             .ToList();

        if (!richestPlayers.Any()) return;

        foreach (var richest in richestPlayers)
        {
            if (richest.Coin < 12) continue;

            int totalGive = 4 * (players.Count - 1);
            int actualTake = richest.LoseCoin(totalGive);

            int perPlayerGain = actualTake / (players.Count - 1);
            foreach (var p in players)
            {
                if (p != richest)
                    p.AddCoin(perPlayerGain);
            }
        }
    }
}