using UnityEngine;
using static Tile;

public class TileView : MonoBehaviour
{
    public MeshRenderer baseRenderer;

    public void Apply(Tile tile)
    {
        Color targetColor;

        switch (tile.Type)
        {
            case TileType.Start:
                targetColor = Color.yellow;
                break;

            case TileType.CoinGain:
                targetColor = new Color(0.6f, 1f, 0.2f);
                break;

            case TileType.CoinLose:
                targetColor = Color.red;
                break;

            case TileType.ItemGain:
                targetColor = Color.green;
                break;

            case TileType.RandomMapEvent:
                targetColor = Color.black;
                break;

            case TileType.Flip:
                targetColor = new Color(0.6f, 0.2f, 1f);
                break;

            default:
                targetColor = Color.white;
                break;
        }

        baseRenderer.material.color = targetColor;
    }
}