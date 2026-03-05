using System;

public class StealCoinItemEffect : IItemEffect
{
    public ItemTargetType TargetType => ItemTargetType.EnemyPlayer;

    private Random random = new Random();

    public void Apply(GameCore game, Player user, Player targetPlayer, TokenGroup targetTokenGroup)
    {
        int coin = random.Next(3, 9);

        int stolen = targetPlayer.LoseCoin(coin);
        user.AddCoin(stolen);
    }
}