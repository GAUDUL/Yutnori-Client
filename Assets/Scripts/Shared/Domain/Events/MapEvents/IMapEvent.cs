public interface IMapEvent
{
    Tile[] Execute(Board board, Tile triggerTile, MapEventSystem system);
}