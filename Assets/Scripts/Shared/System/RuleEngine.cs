using System.Collections.Generic;

public class RuleEngine
{
    public bool ResolveCapture(Token movingToken, Tile tile, List<Player> players)
    {
        bool captured = false;

        foreach (var other in tile.tokens)
        {
            if (other.PlayerId == movingToken.PlayerId) 
                continue;

            Player target = players.Find(p =>  p.PlayerId == other.PlayerId);
            target.LoseCoin(3);

            captured = true;
        }

        return captured;
    }
}
