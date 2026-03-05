
using System;

public class Player
{
    public string PlayerId { get; private set; }
    public int Coin { get; private set; }
    //+ 아이템 등

    public Player(string id)
    {
        PlayerId = id;
        Coin = 30;
    }

    public void AddCoin(int amount)
    {
        if(amount <= 0)
        {
           return;
        }

        Coin += amount;
    }

    public int LoseCoin(int amount)
    {
        if(amount <= 0)
        {
            return 0;
        }

        int lost = Math.Min(Coin, amount);
        Coin -= lost;

        return lost;
    }
}
