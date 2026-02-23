public class RollValidator
{
    public RollResult Validate(bool isRollPhase, int remainingRolls)
    {
        if (!isRollPhase)
            return RollResult.Fail(RollError.InvalidGameState);

        if (remainingRolls <= 0)
            return RollResult.Fail(RollError.NoRemiaingRoll);

        return RollResult.Success(0, false);
    }
}