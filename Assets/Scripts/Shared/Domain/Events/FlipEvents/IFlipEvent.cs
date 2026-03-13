using System.Collections.Generic;

public interface IFlipEvent
{
    void Execute(Player currentPlayer, List<Player> allPlayers);
}