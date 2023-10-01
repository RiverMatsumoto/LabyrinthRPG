using Godot;

public partial class Game : Node
{
    public static GameState State { get; set; }
    public Party CurrentParty { get; set; }

    public override void _Ready()
    {
        CurrentParty = new Party();
        CurrentParty.AddToFrontRow(new Character());
        CurrentParty.AddToFrontRow(new Character());
        CurrentParty.AddToBackRow(new Character());
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
