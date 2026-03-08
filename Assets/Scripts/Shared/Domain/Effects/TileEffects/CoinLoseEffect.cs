using System;

// ÄÚÀÎ ÀÒÀ½ Ä­
public class CoinLoseEffect : ITileEffect
{
    private static readonly Random rng = new Random();
    public void Execute( Player player, Tile tile)
    {
        int coin = rng.Next(1, 4);
        player.LoseCoin(coin);
    }
}