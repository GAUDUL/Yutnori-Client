public class Token
{
    //소유 플레이어 ID
    public string PlayerId { get; private set;}
    // 말 ID
    public string TokenId { get; private set; }

    public Token(string playerId, string tokenId)
    {
        PlayerId = playerId;
        TokenId = tokenId;
    }

}
