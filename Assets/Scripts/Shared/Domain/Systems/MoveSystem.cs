public class MoveSystem
{
    private Board board;

    public MoveSystem(Board board)
    {
        this.board = board;
    }

    public (Tile destination, int lapCount) ExecuteMove(TokenGroup tokenGroup, int step, Player player)
    {
        var (destination, lapCount) = board.MoveTokenGroup(tokenGroup, step);

        if (lapCount > 0)
        {
            int reward = tokenGroup.IsGrouped ? 60 : 40;
            player.AddCoin(reward * lapCount);
        }

        return (destination, lapCount);
    }
}