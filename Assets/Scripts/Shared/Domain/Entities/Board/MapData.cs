using System.Collections.Generic;

public static class MapData
{
    public static Dictionary<int, Tile.TileType> Default()
    {
        return new Dictionary<int, Tile.TileType>()
        {
            { 0, Tile.TileType.Start },
            { 1, Tile.TileType.CoinGain },
            { 2, Tile.TileType.CoinGain },
            { 3, Tile.TileType.CoinGain },
            { 4, Tile.TileType.RandomMapEvent },
            { 5, Tile.TileType.ItemGain },
            { 6, Tile.TileType.CoinLose },
            { 7, Tile.TileType.RandomPlayerEvent },
            { 8, Tile.TileType.CoinGain },
            { 9, Tile.TileType.CoinLose },
            { 10, Tile.TileType.RandomMapEvent },
            { 11, Tile.TileType.CoinGain },
            { 12, Tile.TileType.ItemGain },
            { 13, Tile.TileType.CoinLose },
            { 14, Tile.TileType.RandomPlayerEvent },
            { 15, Tile.TileType.CoinLose },
            { 16, Tile.TileType.ItemGain },
            { 17, Tile.TileType.CoinGain },
            { 18, Tile.TileType.RandomMapEvent },
            { 19, Tile.TileType.RandomPlayerEvent },
        };
    }
}