using System;

// 뒤집 칸 추가
public class AddFlipTilesEvent : IMapEvent
{
    private Random random = new Random();

    public void Execute(Board board, Tile triggerTile, MapEventSystem system)
    {
        BoardEvent e = new BoardEvent();
        e.RemainingTurns = system.PlayerCount;

        int count = 0;
        int safety = 0; // 무한 루프 방지용

        while (count < 3 && safety < 100)
        {
            safety++;

            int index = random.Next(board.TileCount);
            var tile = board.GetTile(index);

            if (tile.OriginalType != null)
                continue;

            // 기존 타입 저장
            tile.OriginalType = tile.Type;
            tile.Type = Tile.TileType.Flip;

            e.ChangedTiles.Add(index);

            count++;
        }

        if (e.ChangedTiles.Count > 0)
            system.AddBoardEvent(e);
    }
}