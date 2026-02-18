public class Token
{
    //소유 플레이어 ID
    public string PlayerId { get; private set;}
    // 말 ID
    public string TokenId { get; private set; }
    //현재 타일 위치
    public int CurrentTileIndex { get; set;}
    //public bool IsGroup {get; private set; }

    public Token(string playerId, string tokenId)
    {
        PlayerId = playerId;
        TokenId = tokenId;
        CurrentTileIndex = 0;
    }

}
