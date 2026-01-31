using Godot;

public sealed partial class GameGlobals : Node
{
    public GameContext GameContext { get; private set; }
    public GameSettings GameSettings { get; private set; }
    public ActionRegistry ActionRegistry { get; private set; }

    public override void _EnterTree()
    {
        CreateNewSave();
        // GameSettings = Util.LoadOrSaveNewSettings();
        GameSettings = GameSettings.GetBaseSettings();
        ActionRegistry = Util.LoadOrSaveNewActionRegistry();
        ActionRegistry.Load();
    }

    public void LoadSave(string id)
    {
        GameContext = Util.LoadSave(id);
    }

    public void CreateNewSave()
    {
        GameContext = new GameContext();
        // Util.Save(GameSave, 0, true);
    }
}
