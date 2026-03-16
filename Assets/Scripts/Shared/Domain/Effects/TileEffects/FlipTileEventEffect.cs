using System;
using System.Collections.Generic;
using System.Linq;

public class FlipTileEventEffect : ITileEffect
{
    private List<IFlipEvent> flipEvents;
    private GameCore gameCore;
    private Random random = new Random();

    public FlipTileEventEffect(GameCore gameCore)
    {
        this.gameCore = gameCore;
        flipEvents = new List<IFlipEvent>()
        {
            new TakeFromRichestEvent(), // 25
            new Coin10GiveOr5TakeEvent(), // 30
            new OneOnOneChallengeEvent(), // 30
            new ReduceAllTwoThirdEvent(), // 10
            new EqualizeAllCoinsEvent(), // 5
        };
    }

    public TileEffectResult Execute(Player player, Tile tile)
    {
        var allPlayers = gameCore.PlayersById.Values.ToList();
        int roll = random.Next(0, 100);

        if (roll < 25)
            flipEvents[0].Execute(player, allPlayers); // TakeFromRichestEvent
        else if (roll < 55)
            flipEvents[1].Execute(player, allPlayers); // Coin10GiveOr5TakeEvent
        else if (roll < 85)
            flipEvents[2].Execute(player, allPlayers); // OneOnOneChallengeEvent
        else if (roll < 95)
            flipEvents[3].Execute(player, allPlayers); // ReduceAllTwoThirdEvent
        else
            flipEvents[4].Execute(player, allPlayers); // EqualizeAllCoinsEvent

        return null;
    }

}