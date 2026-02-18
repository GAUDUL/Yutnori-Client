using UnityEngine;
public class YutSystem
{
    public enum YutResult
    {
        Do = 1,
        Gae = 2,
        Geol = 3,
        Yut = 4,
        Mo = 5
    }

    public YutResult Roll()
    {
        int rand = Random.Range(0, 16);
        if (rand < 4)
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
}
