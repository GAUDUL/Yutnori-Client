
// æ∆¿Ã≈€ »πµÊ ƒ≠
public class ItemGainEffect : ITileEffect
{
    private ItemSystem itemSystem = new ItemSystem();
    public TileEffectResult Execute(Player player, Tile tile)
    {
        var item = itemSystem.GenerateItem();
        player.AddItem(item);

        return null;
    }
}