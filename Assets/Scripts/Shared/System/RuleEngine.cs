using System.Collections.Generic;

public class RuleEngine
{
    public bool ResolveCapture(Token movingToken, Tile tile, Dictionary<string, Player> playersById)
    {
        bool captured = false;
        int totalCoinGained = 0;

        foreach (var other in tile.tokens)
        {
            if (other.PlayerId == movingToken.PlayerId) 
                continue;

            if (playersById.TryGetValue(other.PlayerId, out Player target))
            {
                target.LoseCoin(3);
                totalCoinGained += 3;
                captured = true;
            }
        }

        if (captured && playersById.TryGetValue(movingToken.PlayerId, out Player mover))
        {
            mover.AddCoin(totalCoinGained);
        }

        return captured;
    }
}
