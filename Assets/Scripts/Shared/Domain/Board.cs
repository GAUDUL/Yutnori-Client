public class Board
{
    private Tile[] tiles;

    public Board(int tileCount)
    {
        tiles = new Tile[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            tiles[i] = new Tile(i);
        }
    }

    //말 이동
    public Tile MoveToken(Token token, int step)
    {
        int current = token.CurrentTileIndex;
        int tileCount = tiles.Length;

        int destination = (current + step) % tileCount;

        tiles[current].tokens.Remove(token);
        tiles[destination].tokens.Add(token);

        token.CurrentTileIndex = destination;

        return tiles[destination];
    }

    // 시작 위치에 말 배치
    public void PlaceAtStart(Token token)
    {
        tiles[0].tokens.Add(token);
        token.CurrentTileIndex = 0;
    }

    // 특정 인덱스 타일 반환
    public Tile GetTile(int index)
    {
        return tiles[index];
    }
}