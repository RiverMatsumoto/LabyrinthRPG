using System.Diagnostics;
using Godot;

public partial class RandomBattleTracker : Node
{
    [Export] MapScene player;
    
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    void _on_map_scene_move_ended()
    {
        Debug.Print("Random battle tracker received move ended signal from map scene");
    }
}
