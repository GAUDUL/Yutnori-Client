using System.Collections.Generic;
using System.Linq;

public class RuleEngine
{
    public bool ResolveCapture(TokenGroup movingGroup, Tile tile, Dictionary<string, Player> playersById)
    {
        bool captured = false;

        var others = tile.tokenGroups.ToList();

        foreach (var other in others)
        {
            if (other.PlayerId == movingGroup.PlayerId) 
                continue;

            if (playersById.TryGetValue(other.PlayerId, out Player target) &&
                playersById.TryGetValue(movingGroup.PlayerId, out Player mover))
            {
                // 코인 계산
                int coin = CalculateCaptureCoin(movingGroup, other);

                target.LoseCoin(coin);
                mover.AddCoin(coin);

                // 상대가 업기 상태일 경우, 업기 상태 해제
                if (other.IsGrouped)
                {
                    tile.tokenGroups.Remove(other);

                    // 업기 상태 해제된 상태
                    var splitted = other.Split();

                    // 해제된 각 말들을 각각 하나의 그룹으로 취급하여 저장
                    foreach (var group in splitted)
                    {
                        tile.tokenGroups.Add(group);
                    }
                }

                captured = true;
            }
        }

        return captured;
    }

    // 코인 계산
    private int CalculateCaptureCoin(TokenGroup attacker, TokenGroup defender)
    {
        if (attacker.IsGrouped && defender.IsGrouped)
            return 15;

        if (attacker.IsGrouped)
            return 6;

        if (defender.IsGrouped)
            return 10;

        return 3;
    }

}
