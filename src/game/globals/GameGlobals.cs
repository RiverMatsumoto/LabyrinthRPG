using Godot;

public sealed partial class GameGlobals : Node
{
    public static GameGlobals Instance { get; private set; }

    [Export] public GameSave GameSave { get; private set; }
    [Export] public GameSettings GameSettings { get; private set; }
    [Export] public ActionRegistry ActionRegistry { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        CreateNewSave();
        GameSettings = Util.LoadOrSaveNewSettings();
        ActionRegistry = Util.LoadOrSaveNewActionRegistry();
        ActionRegistry.Load();
    }

    public void LoadSave(string id)
    {
        GameSave = Util.LoadSave(id);
    }

    public void CreateNewSave()
    {
        GameSave = new GameSave();
        Util.Save(GameSave, 0, true);
    }
}
