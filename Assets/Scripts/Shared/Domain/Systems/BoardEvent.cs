using System.Collections.Generic;

public class BoardEvent
{
    public List<int> ChangedTiles = new();
    public int RemainingTurns;

    public Tile[] Tick(Board board)
    {
        RemainingTurns--;

        if (RemainingTurns <= 0)
        {
            return Restore(board);
        }

        return null;
    }

    private Tile[] Restore(Board board)
    {
        List<Tile> tiles = new();

        foreach (var index in ChangedTiles)
        {
            Tile targetTile = board.GetTile(index);
            targetTile.Type = targetTile.OriginalType.Value;
            targetTile.OriginalType = null;

            tiles.Add(targetTile);
        }

        return tiles.ToArray();
    }
}