using Godot;

public partial class Game : Node
{
    public static GameState State { get; set; }

    public override void _Ready()
    {
        State = GameState.Labyrinth;
    }
}

public enum GameState
{
    Labyrinth = 0,
    PauseMenu,
    Town,
    Battle
}
