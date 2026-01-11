using Godot;

public class Battler
{
    public BattlerStats Stats;
    public Texture2D Sprite;
    public bool IsAlive => Stats.Hp > 0;

    public Battler()
    {
        Stats = new BattlerStats();
    }

    public Battler(Texture2D sprite)
    {
        Sprite = sprite;
    }

    public Battler(BattlerBase battlerBase)
    {
        Stats = new BattlerStats(battlerBase);
        Sprite = battlerBase.Sprite;
    }

    public Battler(BattlerStats stats)
    {
        Stats = stats;
    }
}
