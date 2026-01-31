using System;

/*
    will need to deal with versioning at some point for forward compatibility.
    Less updates generally better and I need to keep track of migrating every save file
    to the current/latest update maybe in a switch statement
*/

public class GameContext
{
    public GameState State { get; set; }
    public Party CurrentParty { get; set; }

    public GameContext()
    {
        // new save, temp values
        State = GameState.Labyrinth;
        CurrentParty = new Party();
    }
}

[Serializable]
public enum GameState
{
    Labyrinth = 0,
    PauseMenu,
    Town,
    Battle
}
