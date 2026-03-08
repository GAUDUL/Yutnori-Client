using System;
using System.Collections.Generic;
using System.Linq;

public class Board
{
    private Tile[] tiles;
    public int TileCount => tiles.Length;

    public Board(int tileCount, Dictionary<int, Tile.TileType> tileTypeMap)
    {
        tiles = new Tile[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            tiles[i] = new Tile(i, this);

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
    public (Tile tile, int lapCount) MoveTokenGroup(TokenGroup tokenGroup, int step)
    {
        int current = tokenGroup.CurrentTileIndex;
        int tileCount = tiles.Length;

        int destination = ((current + step) % tileCount + tileCount) % tileCount;

        int lapCount = 0;

        if (step > 0)
        {
            int totalMove = current + step;
            lapCount = totalMove / tileCount;
        }

        if (current != destination)
        {
            tiles[current].tokenGroups.Remove(tokenGroup);
            tiles[destination].tokenGroups.Add(tokenGroup);
        }

        tokenGroup.CurrentTileIndex = destination;

        return (tiles[destination], lapCount);
    }

    // 맵 기믹: 연결된 칸으로 이동
    public Tile TeleportTokenGroup(TokenGroup tokenGroup, int destinationIndex)
    {
        int current = tokenGroup.CurrentTileIndex;

        if (current != destinationIndex)
        {
            tiles[current].tokenGroups.Remove(tokenGroup);
            tiles[destinationIndex].tokenGroups.Add(tokenGroup);
        }

        tokenGroup.CurrentTileIndex = destinationIndex;

        return tiles[destinationIndex];
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