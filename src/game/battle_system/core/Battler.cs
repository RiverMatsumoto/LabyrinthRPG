using System.Collections.Generic;
using Godot;

public partial class Battler(BattlerBase battlerBase) : RefCounted
{
    public BattlerStats Stats { get; set; } = new BattlerStats(battlerBase);
    public Texture2D Sprite { get; set; } = battlerBase.Sprite;
    public List<string> Skills { get; set; }
    public bool IsAlive => Stats.Hp.Value > 0;

    // public Battler(BattlerStats stats)
    // {
    //     Stats = stats;
    // }
}
