using Godot;

[GlobalClass]
public sealed partial class BattlerBase : Resource
{
    [Export] public string Name { get; set; }
    [Export] public string Class { get; set; }
    [Export] public string Description { get; set; }
    [Export] public int Level { get; set; }
    [Export] public int Experience { get; set; }
    [Export] public int MaxHp { get; set; }
    [Export] public int Hp { get; set; }
    [Export] public int MaxTp { get; set; }
    [Export] public int Tp { get; set; }
    [Export] public int Str { get; set; }
    [Export] public int Tec { get; set; }
    [Export] public int Agi { get; set; }
    [Export] public int Vit { get; set; }
    [Export] public int Wis { get; set; }
    [Export] public int Luc { get; set; }
    [Export] public Texture2D Sprite { get; set; }


    public BattlerBase()
    {
        Class = "Default";
        Description = "Default description";
        Level = 1;
        Experience = 0;
        Hp = 20;
        MaxHp = 20;
        Tp = 20;
        MaxTp = 20;
        Str = 1;
        Tec = 1;
        Agi = 1;
        Vit = 1;
        Wis = 1;
        Luc = 1;
    }
}
