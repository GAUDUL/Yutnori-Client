// Ä­ ¿¬°á
class ConnectTilesEvent : IMapEvent
{
    public void Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        int tileCount = board.TileCount;
        int index = triggerTile.tileIndex;

        int left = ((index - 3) % tileCount + tileCount) % tileCount;
        int right = ((index + 3) % tileCount + tileCount) % tileCount;

        var leftTile = board.GetTile(left);
        var rightTile = board.GetTile(right);

        if (leftTile.ConnectedTileIndex.HasValue || rightTile.ConnectedTileIndex.HasValue)
            return;

        leftTile.ConnectedTileIndex = right;
        rightTile.ConnectedTileIndex = left;
    }
}