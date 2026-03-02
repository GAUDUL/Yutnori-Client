using System.Collections.Generic;

public class Board
{
    private Tile[] tiles;

    public Board(int tileCount, Dictionary<int, Tile.TileType> tileTypeMap)
    {
        tiles = new Tile[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            tiles[i] = new Tile(i);

            if(tileTypeMap.TryGetValue(i, out var type))
                tiles[i].Type= type;
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

    // 특정 인덱스 타일 반환
    public Tile GetTile(int index)
    {
        return tiles[index];
    }

    // 모든 타일 반환
    public Tile[] GetTiles()
    {
        return tiles;
    }
}