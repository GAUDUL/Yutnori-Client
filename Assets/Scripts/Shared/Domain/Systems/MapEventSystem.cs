using System;
using System.Collections.Generic;

public class MapEventSystem
{
    private TurnManager turnManager;

    private Random random = new Random();
    private List<MapEventEntry> events = new();
    private List<BoardEvent> activeEvents = new();

    public MapEventSystem(TurnManager turnManager)
    {
        this.turnManager = turnManager;

        events.Add(new MapEventEntry(new SwapGainLoseEvent(), 20));
        events.Add(new MapEventEntry(new ConnectTilesEvent(), 40)); 
        events.Add(new MapEventEntry(new ReverseTileEvent(), 30));
        events.Add(new MapEventEntry(new AddFlipTilesEvent(), 10));
    }

    public int PlayerCount => turnManager.PlayerCount;

    public void Execute(Board board, Tile triggerTile)
    {
        int roll = random.Next(0, 100);
        int sum = 0;

        foreach (var entry in events)
        {
            sum += entry.Probability;

            if (roll < sum)
            {
                entry.Event.Execute(board, triggerTile, this);
                return;
            }
        }
    }
    public void AddBoardEvent(BoardEvent e)
    {
        activeEvents.Add(e);
    }

    // 턴 종료 시 호출
    public void TickEvents(Board board)
    {
        for (int i = activeEvents.Count - 1; i >= 0; i--)
        {
            activeEvents[i].Tick(board);

            if (activeEvents[i].RemainingTurns <= 0)
            {
                activeEvents.RemoveAt(i);
            }
        }
    }
}