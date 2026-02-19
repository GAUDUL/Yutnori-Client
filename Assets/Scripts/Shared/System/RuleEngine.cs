using System.Collections.Generic;

public class RuleEngine
{
    public bool ResolveCapture(Token movingToken, Tile tile, Dictionary<string, Player> playersById)
    {
        bool captured = false;

        foreach (var other in tile.tokens)
        {
            if (other.PlayerId == movingToken.PlayerId) 
                continue;

            if (playersById.TryGetValue(other.PlayerId, out Player target))
            {
                target.LoseCoin(3);
                captured = true;
            }
        }

        return captured;
    }
}
