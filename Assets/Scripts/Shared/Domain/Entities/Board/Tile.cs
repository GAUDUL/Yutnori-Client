using System.Collections.Generic;
public class Tile
{
    public enum TileType
    {
        None,
        Start, // 시작
        CoinGain, // 코인 획득
        CoinLose, // 코인 차감
        ItemGain, // 아이템
        RandomMapEvent, // 맵 기믹
        RandomPlayerEvent // 뒤집
    }

    public TileType Type;
    public int tileIndex { get; private set; }
    public List<TokenGroup> tokenGroups = new List<TokenGroup>();

    public Tile(int index)
    {
        tileIndex = index;
    }
}