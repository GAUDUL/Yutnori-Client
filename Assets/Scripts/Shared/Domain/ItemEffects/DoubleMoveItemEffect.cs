public class DoubleMoveItemEffect : IItemEffect
{
    public ItemTargetType TargetType => ItemTargetType.None;
    public void Apply(GameCore game, Player user, Player targetPlayer, TokenGroup targetTokenGroup)
    {
        game.EnableDoubleMove();
    }
}