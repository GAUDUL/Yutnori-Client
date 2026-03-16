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
        events.Add(new MapEventEntry(new ConnectTilesEvent(), 40)); // 연결 O
        events.Add(new MapEventEntry(new ReverseTileEvent(), 30));
        events.Add(new MapEventEntry(new AddFlipTilesEvent(), 10));
    }

    public int PlayerCount => turnManager.PlayerCount;

    public TileEffectResult Execute(Board board, Tile triggerTile)
    {
        int roll = random.Next(0, 100);
        int sum = 0;

        bool isConnected = false;
        int cnt = 0;

        foreach (var entry in events)
        {
            sum += entry.Probability;

            if (roll < sum)
            {
                if (cnt == 1)
                    isConnected = true;

                var changedTiles = entry.Event.Execute(board, triggerTile, this);

                    return new TileEffectResult(changedTiles, isConnected);
            }

            cnt++;
        }

        return null;
    }

    public void AddBoardEvent(BoardEvent e)
    {
        activeEvents.Add(e);
    }

    // 턴 종료 시 호출
    public Tile[] TickEvents(Board board)
    {
        List<Tile> changedTiles = new();

        for (int i = activeEvents.Count - 1; i >= 0; i--)
        {
            var tiles = activeEvents[i].Tick(board);

            if (tiles != null)
            {
                changedTiles.AddRange(tiles);
            }

            if (activeEvents[i].RemainingTurns <= 0)
            {
                activeEvents.RemoveAt(i);
            }
        }

        return changedTiles.Count > 0 ? changedTiles.ToArray() : null;
    }
}