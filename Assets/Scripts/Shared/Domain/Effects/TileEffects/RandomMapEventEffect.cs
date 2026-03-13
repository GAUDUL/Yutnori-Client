
// ¸Ê ±â¹Í Ä­
public class RandomMapEventEffect : ITileEffect
{
    private MapEventSystem mapEventSystem;

    public RandomMapEventEffect(MapEventSystem mapEventSystem)
    {
        this.mapEventSystem = mapEventSystem;
    }

    public void Execute(Player player, Tile tile)
    {
        mapEventSystem.Execute(tile.board, tile);
    }
}