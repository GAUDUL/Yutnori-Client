using System;

// ÄÚÀÎ È¹µæ Ä­
public class CoinGainEffect : ITileEffect
{
    private static readonly Random rng = new Random();
    public void Execute( Player player, Tile tile)
    {
        int coin = rng.Next(1, 4);
        player.AddCoin(coin);
    }
}