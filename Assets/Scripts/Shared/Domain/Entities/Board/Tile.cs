using System.Collections.Generic;
public class Tile
{
    public int tileIndex { get; private set; }
    public List<TokenGroup> tokenGroups = new List<TokenGroup>();

    public Tile(int index)
    {
        tileIndex = index;
    }
}