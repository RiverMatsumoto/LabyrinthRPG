using System.Collections.Generic;
using Godot;

public partial class Battler : RefCounted
{
    public BattlerStats Stats;
    public Texture2D Sprite;
    public List<string> Skills;
    public bool IsAlive => Stats.Hp.Value > 0;

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
