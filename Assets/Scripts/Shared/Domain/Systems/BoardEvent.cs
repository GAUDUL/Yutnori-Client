using System.Collections.Generic;

public class BoardEvent
{
    public List<int> ChangedTiles = new();
    public int RemainingTurns;

    public void Tick(Board board)
    {
        RemainingTurns--;

        if (RemainingTurns <= 0)
        {
            Restore(board);
        }
    }

    private void Restore(Board board)
    {
        foreach (var index in ChangedTiles)
        {
            Tile targetTile = board.GetTile(index);
            targetTile.Type = targetTile.OriginalType.Value;
            targetTile.OriginalType = null;
        }
    }
}