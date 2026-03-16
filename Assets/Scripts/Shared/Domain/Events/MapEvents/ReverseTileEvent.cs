using NUnit.Framework;
using System;
using System.Collections.Generic;

// ¸ğµç È¹µæ Ä­ => ÀÒÀ½ Ä­ or ÀÒÀ½ Ä­ => È¹µæ Ä­
public class ReverseTileEvent : IMapEvent
{
    private Random random = new Random();

    public Tile[] Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        // ¹Ù²ï Å¸ÀÏµé ÀúÀå
        List<Tile> list = new List<Tile>();

        BoardEvent e = new BoardEvent();
        e.RemainingTurns = system.PlayerCount + 1;

        bool gainToLose = random.Next(0, 2) == 0;

        foreach (var tile in board.GetTiles())
        {
            if (tile.OriginalType != null)
                continue;

            // ¸ğµç È¹µæ Ä­ => ÀÒÀ½ Ä­
            if (gainToLose && tile.Type == Tile.TileType.CoinGain)
            {
                tile.OriginalType = tile.Type;
                tile.Type = Tile.TileType.CoinLose;

                e.ChangedTiles.Add(tile.tileIndex);
                list.Add(tile);
            }
            // ¸ğµç ÀÒÀ½ Ä­ => È¹µæ Ä­
            else if (!gainToLose && tile.Type == Tile.TileType.CoinLose)
            {
                tile.OriginalType = tile.Type;
                tile.Type = Tile.TileType.CoinGain;

                e.ChangedTiles.Add(tile.tileIndex);
                list.Add(tile);
            }
        }

        if (e.ChangedTiles.Count > 0)
            system.AddBoardEvent(e);

        return list.ToArray();
    }
}