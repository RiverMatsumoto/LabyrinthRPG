using System.Collections.Generic;
using System.Linq;
using Godot;

public enum EquipSlot
{
    Weapon,
    Chest,
    Accessory1,
    Accessory2
}

public enum StatId
{
    Hp,
    Tp,
    Str,
    Tec,
    Agi,
    Vit,
    Luc
}
public readonly record struct StatModifier(StatId Stat, int FlatDelta);

[GlobalClass]
public partial class EquipModule : ItemModule
{
    [Export] public EquipSlot Slot { get; set; } = EquipSlot.Accessory1;
    [Export] public bool UniqueEquipped { get; set; } = false;
}

[GlobalClass]
public partial class StatModifiersModule : ItemModule
{
    [Export] public Godot.Collections.Array<StatModifierResource> Modifiers { get; set; } = new();

    public IEnumerable<StatModifier> Enumerate()
        => Modifiers.Select(m => new StatModifier(m.Stat, m.FlatDelta));
}

[GlobalClass]
public partial class StatModifierResource : Resource
{
    [Export] public StatId Stat { get; set; } = StatId.Str;
    [Export] public int FlatDelta { get; set; } = 0;
}

[GlobalClass]
public partial class WeaponModule : ItemModule
{
    [Export] public int AttackPower { get; set; } = 10;
    [Export] public string DamageType { get; set; } = "Physical";
}
