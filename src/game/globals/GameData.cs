using Godot;

[GlobalClass]
public partial class GameData : Resource
{
    [Export] public GameState State { get; set; }
    public Party CurrentParty { get; set; }
}


public enum GameState
{
    Labyrinth = 0,
    PauseMenu,
    Town,
    Battle
}
