using Godot;
using Godot.Collections;

public enum UseContext
{
    Labyrinth, // overworld exploration
    Town,
    Battle,
    Menu
}

public enum Targeting
{
    None,
    Self,
    SingleAlly,
    SingleEnemy,
    RowAllies,
    RowEnemies,
    AllAllies,
    AllEnemies
}

public enum ItemCategory
{
    Consumable,
    Weapon,
    Armor,
    KeyItem,
    Material,
    Misc
}

[GlobalClass]
public partial class ItemDef : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";
    [Export] public ItemCategory Category { get; set; } = ItemCategory.Misc;
    // Modules add capabilities (equip, use, stat mods, weapon info, etc.)
    [Export] public Array<ItemModule> Modules { get; set; } = new();

    public bool Has<T>() where T : ItemModule => Get<T>() != null;

    public T Get<T>() where T : ItemModule
    {
        foreach (var m in Modules)
        {
            if (m is T t) return t;
        }
        return null;
    }
}

public abstract partial class ItemModule : Resource { }
