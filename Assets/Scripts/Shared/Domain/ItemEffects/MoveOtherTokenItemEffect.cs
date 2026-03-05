using System;

public class MoveOtherTokenItemEffect : IItemEffect
{
    public ItemTargetType TargetType => ItemTargetType.EnemyToken;

    private Random random = new Random();

    // 플레이어 선택 로직 + 잡기 반영 x 이동 로직 추가
    public void Apply(GameCore game, Player user, Player targetPlayer, TokenGroup targetTokenGroup)
    {
        int step = random.Next(0, 2) == 0 ? -1 : 1;

        game.MoveTokenWithoutCapture(targetTokenGroup, step, targetPlayer);
    }
}