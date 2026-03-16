
// ¸Ê ±â¹Í Ä­
public class RandomMapEventEffect : ITileEffect
{
    private MapEventSystem mapEventSystem;

    public RandomMapEventEffect(MapEventSystem mapEventSystem)
    {
        this.mapEventSystem = mapEventSystem;
    }

    public TileEffectResult Execute(Player player, Tile tile)
    {
        return mapEventSystem.Execute(tile.board, tile);
    }
}