// È¹µæ Ä­ <-> ÀÒÀ½ Ä­
using System.Collections.Generic;

class SwapGainLoseEvent : IMapEvent
{
    public Tile[] Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        List<Tile> list = new List<Tile>();

        foreach (var tile in board.GetTiles())
        {
            if (tile.Type == Tile.TileType.CoinGain)
                tile.Type = Tile.TileType.CoinLose;

            else if (tile.Type == Tile.TileType.CoinLose)
                tile.Type = Tile.TileType.CoinGain;

            list.Add(tile);
        }

        return list.ToArray();
    }
}