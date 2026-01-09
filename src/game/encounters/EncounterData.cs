using Godot;

[GlobalClass]
public partial class EncounterData : Resource
{
    [Export] public Godot.Collections.Array<string> Enemies { get; set; } = new();
    [Export] public int Floor { get; set; }
}
