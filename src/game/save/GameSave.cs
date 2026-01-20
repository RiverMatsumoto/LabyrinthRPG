using Godot;

[GlobalClass]
public partial class GameSave : Resource
{
    [Export] public GameState State { get; set; }
    public BattlePlaybackOptions PlaybackOptions { get; set; }
    public Party CurrentParty { get; set; }

    public GameSave()
    {
        // new save, temp values
        State = GameState.Labyrinth;
        PlaybackOptions = new BattlePlaybackOptions();
        CurrentParty = new Party();
    }
}


public enum GameState
{
    Labyrinth = 0,
    PauseMenu,
    Town,
    Battle
}
