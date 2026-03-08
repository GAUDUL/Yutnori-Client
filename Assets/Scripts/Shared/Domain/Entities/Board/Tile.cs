using System.Collections.Generic;
public class Tile
{
    public enum TileType
    {
        None,
        Start, // ½ÃÀÛ
        CoinGain, // ÄÚÀÎ È¹µæ
        CoinLose, // ÄÚÀÎ Â÷°¨
        ItemGain, // ¾ÆÀÌÅÛ
        RandomMapEvent, // ¸Ê ±â¹Í
        Flip // µÚÁı
    }

    public TileType Type;
    public TileType? OriginalType;

    public Board board {  get; private set; }
    public int tileIndex { get; private set; }
    public List<TokenGroup> tokenGroups = new List<TokenGroup>();
    public int? ConnectedTileIndex { get; set; } // ¿¬°áµÈ Ä­

    public Tile(int index, Board board)
    {
        tileIndex = index;
        this.board = board;
    }
}