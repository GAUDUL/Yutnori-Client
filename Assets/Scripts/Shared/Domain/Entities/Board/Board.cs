using System.Collections.Generic;

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

    // 전체 그룹 조회
    public IEnumerable<TokenGroup> GetAllGroups() 
    {
        foreach (var tile in tiles)
        {
            foreach (var group in tile.tokenGroups)
            {
                yield return group;
            }
        }
    }

    // 초기 그룹 생성
    public TokenGroup CreateInitialGroup(Token token)
    {
        var group = new TokenGroup(token.PlayerId, 0, token);
        tiles[0].tokenGroups.Add(group);
        return group;
    }

    //말 이동
    public Tile MoveTokenGroup(TokenGroup tokenGroup, int step)
    {
        int current = tokenGroup.CurrentTileIndex;
        int tileCount = tiles.Length;

        int destination = ((current + step) % tileCount + tileCount) % tileCount;

        tiles[current].tokenGroups.Remove(tokenGroup);
        tiles[destination].tokenGroups.Add(tokenGroup);

        tokenGroup.CurrentTileIndex = destination;

        return tiles[destination];
    }

    //// 시작 위치에 말 배치
    //public void PlaceAtStart(TokenGroup tokenGroup)
    //{
    //    tiles[0].tokenGroups.Add(tokenGroup);
    //    tokenGroup.CurrentTileIndex = 0;
    //}

    // 특정 인덱스 타일 반환
    public Tile GetTile(int index)
    {
        return tiles[index];
    }
}