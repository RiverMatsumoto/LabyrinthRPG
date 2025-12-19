using Godot;

public partial class CharacterDef : Resource
{
    [Export] public string Name { get; set; }
    [Export] public string Class { get; set; }
    [Export] public int Level { get; set; }
    [Export] public int Experience { get; set; }
    [Export] public int Hp { get; set; }
    [Export] public int Tp { get; set; }
    [Export] public int Strength { get; set; }

    public CharacterDef()
    {
        Name = "Default";
        Class = "Default";
        Level = 1;
        Experience = 0;
        Hp = 10;
        Tp = 10;
        Strength = 1;
    }
}
