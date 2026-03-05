public interface IItemEffect
{
    ItemTargetType TargetType { get; }
    void Apply(GameCore game, Player user, Player targetPlayer, TokenGroup targetTokenGroup);
}