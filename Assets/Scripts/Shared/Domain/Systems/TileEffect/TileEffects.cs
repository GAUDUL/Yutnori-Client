using System.Collections.Generic;

public static class TileEffects
{
    public static Dictionary<Tile.TileType, ITileEffect> Default()
    {
        return new Dictionary<Tile.TileType, ITileEffect>()
        {
            {Tile.TileType.CoinGain, new CoinGainEffect() },
            {Tile.TileType.CoinLose, new CoinLoseEffect() },
        };
    }
}