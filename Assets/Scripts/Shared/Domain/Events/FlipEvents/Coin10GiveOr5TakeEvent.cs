using System;
using System.Collections.Generic;
using System.Linq;


// 코인 10개 주기 or 5개 뺏기 (대상: poorest)
public class Coin10GiveOr5TakeEvent : IFlipEvent
{
    private Random random = new Random();

    public void Execute(Player currentPlayer, List<Player> players)
    {
        var poorest = players
                           // 오름차순 정렬
                          .OrderBy(p => p.Coin)
                          .FirstOrDefault();

        if (poorest == null || poorest.PlayerId == currentPlayer.PlayerId) return;

        int subRoll = random.Next(0, 10);

        // 코인 주기
        if (subRoll < 7)
        {
            int actualTake = currentPlayer.LoseCoin(10);
            poorest.AddCoin(actualTake);
        }
        // 코인 뺏기
        else
        {
            int actualTake = poorest.LoseCoin(5);
            currentPlayer.AddCoin(actualTake);
        }
    }
}
