using System;

// ¸ðµç È¹µæ Ä­ => ÀÒÀ½ Ä­ or ÀÒÀ½ Ä­ => È¹µæ Ä­
public class ReverseTileEvent : IMapEvent
{
    private Random random = new Random();

    public void Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        BoardEvent e = new BoardEvent();
        e.RemainingTurns = system.PlayerCount;

        bool gainToLose = random.Next(0, 2) == 0;

        foreach (var tile in board.GetTiles())
        {
            if (tile.OriginalType != null)
                continue;

            // ¸ðµç È¹µæ Ä­ => ÀÒÀ½ Ä­
            if (gainToLose && tile.Type == Tile.TileType.CoinGain)
            {
                tile.OriginalType = tile.Type;
                tile.Type = Tile.TileType.CoinLose;

                e.ChangedTiles.Add(tile.tileIndex);
            }
            // ¸ðµç ÀÒÀ½ Ä­ => È¹µæ Ä­
            else if (!gainToLose && tile.Type == Tile.TileType.CoinLose)
            {
                tile.OriginalType = tile.Type;
                tile.Type = Tile.TileType.CoinGain;

                e.ChangedTiles.Add(tile.tileIndex);
            }
        }

        if (e.ChangedTiles.Count > 0)
            system.AddBoardEvent(e);
    }
}