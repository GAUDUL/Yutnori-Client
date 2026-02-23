
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

    public void LoseCoin(int amount)
    {
        if(amount <= 0)
        {
            return;
        }

        Coin -= amount;

        if(Coin < 0)
        {
            Coin = 0;
        }
    }
}
