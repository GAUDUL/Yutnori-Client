public interface IMapEvent
{
    void Execute(Board board, Tile triggerTile, MapEventSystem system);
}