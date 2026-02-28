using System.Collections.Generic;
using System;

public class TokenGroup
{
    public string GroupId { get; private set; }
    public string PlayerId { get; private set; }
    public int CurrentTileIndex { get; set; }

    // 각 토큰을 하나의 그룹으로 취급
    public List<Token> Tokens { get; private set; } = new();

    public bool IsGrouped => Tokens.Count > 1;

    public TokenGroup(string playerId, int tileIndex, Token token)
    {
        PlayerId = playerId;
        GroupId = $"{playerId}_Group_{Guid.NewGuid():N}";
        CurrentTileIndex = tileIndex;
        Tokens.Add(token);
    }

    // 업기
    public void Merge(TokenGroup other)
    {
        if (other == null) return;
        if (other.PlayerId != PlayerId)
            throw new InvalidOperationException("다른 플레이어와 업기 불가능");

        Tokens.AddRange(other.Tokens);
        other.Tokens.Clear();
    }

    // 업기 상태 해제
    public List<TokenGroup> Split()
    {
        var result = new List<TokenGroup>();

        foreach (var token in Tokens)
        {
            result.Add(new TokenGroup(token.PlayerId, CurrentTileIndex, token));
        }
        //Tokens.Clear();
        return result;
    }
}