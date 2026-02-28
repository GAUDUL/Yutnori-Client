using System;
public class YutSystem
{
    public enum YutResult
    {
        BackDo = -1,
        Do = 1,
        Gae = 2,
        Geol = 3,
        Yut = 4,
        Mo = 5
    }

    private static readonly Random rng = new Random();
    public YutResult Roll()
    {
        int rand = rng.Next(0, 16);
        if (rand < 1)
            return YutResult.BackDo;
        else if (rand < 4)
            return YutResult.Do;
        else if (rand < 10)
            return YutResult.Gae;
        else if (rand < 14)
            return YutResult.Geol;
        else if (rand < 15)
            return YutResult.Yut;
        else
            return YutResult.Mo;
    }

    // À· or ¸ð È®ÀÎ
    public bool IsExtraTurn(YutResult result)
    {
        return result == YutResult.Yut || result == YutResult.Mo;
    }
}
