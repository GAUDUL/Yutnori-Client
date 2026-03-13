public class BackDoItemEffect : IItemEffect
{
    public ItemTargetType TargetType => ItemTargetType.MyToken;
    // 바로 뒤로 이동하도록 함수 만들기 + 말 선택하기
    public void Apply(GameCore game, Player player, Player targetPlayer, TokenGroup targetTokenGroup)
    {
        string tokenId = targetTokenGroup.Tokens[0].TokenId;

        game.AddStep(-1);

        game.SelectToken(tokenId);
        game.Move(-1);
    }
}