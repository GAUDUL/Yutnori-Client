public class TileEffectResult
{
    public Tile[] ChangedTiles;
    public bool IsConnected;

    public TileEffectResult(Tile[] changedTiles, bool isConnected)
    {
        ChangedTiles = changedTiles;
        this.IsConnected = isConnected;
    }
}