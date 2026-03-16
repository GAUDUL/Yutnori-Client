using UnityEngine;
public class BoardView : MonoBehaviour
{
    [SerializeField] private TileView[] tileViews;
    public int TileCount => tileViews.Length;
    public Vector3 GetWorldPosition(int index)
    {
        return tileViews[index].transform.position;
    }
    public void ApplyAllTiles(Tile[] tiles)
    {
        for (int i = 0; i < tileViews.Length; i++)
        {
            tileViews[i].Apply(tiles[i]);
        }
    }

    public void ApplySomeTiles(Tile[] tiles)
    {
        foreach (Tile tile in tiles)
        {
            tileViews[tile.tileIndex].Apply(tile);
        }
    }
}
