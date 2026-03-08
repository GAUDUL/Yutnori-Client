using System.Collections.Generic;

// Ä­ Á¾·ù
public static class TileEffects
{
    public static Dictionary<Tile.TileType, ITileEffect> Default(MapEventSystem mapEventSystem)
    {
        return new Dictionary<Tile.TileType, ITileEffect>()
        {
            {Tile.TileType.CoinGain, new CoinGainEffect() },
            {Tile.TileType.CoinLose, new CoinLoseEffect() },
            {Tile.TileType.ItemGain, new ItemGainEffect() },
            {Tile.TileType.RandomMapEvent, new  RandomMapEventEffect(mapEventSystem)}
        };
    }
}