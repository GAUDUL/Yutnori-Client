using System.Collections.Generic;
using UnityEngine;

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
        GroupId = $"{PlayerId}_Group";
        CurrentTileIndex = tileIndex;
        Tokens.Add(token);
    }

    // 업기
    public void Merge(TokenGroup other)
    {
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

        return result;
    }
}