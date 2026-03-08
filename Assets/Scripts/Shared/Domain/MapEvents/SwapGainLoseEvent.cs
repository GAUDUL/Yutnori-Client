// È¹µæ Ä­ <-> ÀÒÀ½ Ä­
class SwapGainLoseEvent : IMapEvent
{
    public void Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        foreach (var tile in board.GetTiles())
        {
            if (tile.Type == Tile.TileType.CoinGain)
                tile.Type = Tile.TileType.CoinLose;

            else if (tile.Type == Tile.TileType.CoinLose)
                tile.Type = Tile.TileType.CoinGain;
        }
    }
}