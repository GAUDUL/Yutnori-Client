using System.Collections.Generic;

public class TurnManager
{
    private List<string> playerOrder;
    public int CurrentTurnIndex { get; private set; } = 0;

    public int RemainingRolls { get; private set; } = 1;
    public bool UsedYutExtraTurn { get; private set; } = false;
    public bool UsedCaptureExtraTurn { get; private set; } = false;

    public string CurrentPlayerId => playerOrder[CurrentTurnIndex];

    public TurnManager(List<string> playerOrder)
    {
        this.playerOrder = playerOrder;
    }
    public int PlayerCount => playerOrder.Count;

    public void UseRoll()
    {
        RemainingRolls--;
    }

    // 윷 or 모 추가 턴 부여
    public void GrantExtraTurnYut()
    {
        if (!UsedYutExtraTurn)
        {
            RemainingRolls++;
            UsedYutExtraTurn = true;
        }
    }

    // 말 잡기 추가 턴 부여
    public void GrantExtraTurnCapture()
    {
        if (!UsedCaptureExtraTurn)
        {
            RemainingRolls++;
            UsedCaptureExtraTurn = true;
        }
    }

    public bool EndTurn()
    {
        CurrentTurnIndex = (CurrentTurnIndex + 1) % playerOrder.Count;
        bool isRoundEnd = CurrentTurnIndex == 0;

        RemainingRolls = 1;
        UsedYutExtraTurn = false;
        UsedCaptureExtraTurn = false;

        return isRoundEnd;
    }
}