using System.Collections.Generic;
using static Tile;

public class TileEffectSystem
{
    private Dictionary<TileType, ITileEffect> effects;

    public TileEffectSystem()
    {
        effects = TileEffects.Default();
    }

    public void Execute(Tile tile, Player player)
    {
        if (effects.TryGetValue(tile.Type, out var effect))
        {
            effect.Execute(player, tile);
        }
    }
}