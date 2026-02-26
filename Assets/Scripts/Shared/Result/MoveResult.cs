using System.Collections.Generic;

public enum MoveError
{
    None,
    NotYourTurn,
    NotYourToken,
    NoStep,
    InvalidToken,
    InvalidStep,
    InvalidGameState
}

public class MoveResult
{
    public MoveError Error { get; }
    public string GroupId { get; } 
    public List<string> MovedTokenIds { get; }
    public int NewIndex { get; }
    public string CurrentTurnPlayerId { get; }
    public bool Captured { get; }
    public bool IsRoundEnd { get; }
    public bool NeedMerge { get; }

    public bool IsSuccess => Error == MoveError.None;

    private MoveResult(
        MoveError error,
        string groupId = null,
        List<string> movedTokenIds = null,
        int newIndex = 0,
        string currentTurnPlayerId = null,
        bool captured = false,
        bool isRoundEnd = false,
        bool needMerge = false)
    {
        Error = error;
        GroupId = groupId;
        MovedTokenIds = movedTokenIds;
        NewIndex = newIndex;
        CurrentTurnPlayerId = currentTurnPlayerId;
        Captured = captured;
        IsRoundEnd = isRoundEnd;
        NeedMerge = needMerge;
     }

    public static MoveResult Fail(MoveError error)
    {
        return new MoveResult(error);
    }

    public static MoveResult Success(
        string groupId,
        List<string> movedTokenIds,
        int newIndex,
        string currentTurnPlayerId,
        bool captured,
        bool isRoundEnd,
        bool needMerge)
    {
        return new MoveResult(
            MoveError.None,
            groupId,
            movedTokenIds,
            newIndex,
            currentTurnPlayerId,
            captured,
            isRoundEnd,
            needMerge);
    }
}
