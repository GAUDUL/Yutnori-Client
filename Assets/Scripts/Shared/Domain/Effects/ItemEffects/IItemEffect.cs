public interface IItemEffect
{
    ItemTargetType TargetType { get; }
    void Apply(GameCore game, Player player, Player targetPlayer, TokenGroup targetTokenGroup);
}