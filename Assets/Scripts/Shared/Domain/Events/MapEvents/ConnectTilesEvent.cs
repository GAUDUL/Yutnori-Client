// Ä­ ¿¬°á
class ConnectTilesEvent : IMapEvent
{
    public Tile[] Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        Tile[] tiles = new Tile[2];

        int tileCount = board.TileCount;
        int index = triggerTile.tileIndex;

        int left = ((index - 3) % tileCount + tileCount) % tileCount;
        int right = ((index + 3) % tileCount + tileCount) % tileCount;

        var leftTile = board.GetTile(left);
        var rightTile = board.GetTile(right);

        if (leftTile.ConnectedTileIndex.HasValue || rightTile.ConnectedTileIndex.HasValue)
            return null;

        leftTile.ConnectedTileIndex = right;
        rightTile.ConnectedTileIndex = left;

        tiles[0] = leftTile;
        tiles[1] = rightTile;

        return tiles;
    }
}