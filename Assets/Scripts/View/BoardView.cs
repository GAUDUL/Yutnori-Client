using UnityEngine;
public class BoardView : MonoBehaviour
{
    [SerializeField] private TileView[] tileViews;
    public int TileCount => tileViews.Length;
    public Vector3 GetWorldPosition(int index)
    {
        return tileViews[index].transform.position;
    }

}
