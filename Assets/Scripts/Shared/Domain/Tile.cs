using System.Collections.Generic;
public class Tile
{
    public int tileIndex { get; private set; }
    public List<Token> tokens = new List<Token>();

    public Tile(int index)
    {
        tileIndex = index;
    }
}