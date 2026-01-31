using Godot;

public static class Util
{
    public static string ReadonlyPath(string path) => Godot.ProjectSettings.GlobalizePath("res://" + path);

    public static string UserPath(string path) => Godot.ProjectSettings.GlobalizePath("user://" + path);

    /// <summary>
    /// A utility to simplify get the autoloaded globals for this project.
    /// </summary>
    public static GameGlobals GetGameGlobals(Node host) => host.GetTree().GetFirstNodeInGroup("game_globals") as GameGlobals;

    // public static void SaveSettings(GameSettings settings) => ResourceSaver.Save(settings, UserPath("settings.tres"));

    // public static void ResetSettings() => SaveSettings(GameSettings.GetBaseSettings());

    // public static GameSettings LoadOrSaveNewSettings()
    // {
    //     string settingsPath = UserPath("settings.tres");
    //     GD.Print("Loading settings from path:", settingsPath);
    //     if (FileAccess.FileExists(settingsPath))
    //     {
    //         return ResourceLoader.Load<GameSettings>(settingsPath);
    //     }
    //     var settings = GameSettings.GetBaseSettings();
    //     SaveSettings(settings);
    //     return settings;
    // }

    // public static void Save(GameSave gameSave, int id, bool overwrite = true)
    // {
    //     while (true)
    //     {
    //         string savePath = UserPath($"game_save_{id}.tres");
    //         if (FileAccess.FileExists(savePath) && !overwrite)
    //         {
    //             id += 1;
    //         }
    //         else
    //         {
    //             ResourceSaver.Save(gameSave, savePath);
    //             return;
    //         }
    //     }
    // }

    public static GameContext LoadSave(string id)
    {
        return ResourceLoader.Load<GameContext>(UserPath($"game_save_{id}.tres"));
    }

    public static ActionRegistry LoadOrSaveNewActionRegistry()
    {
        return ResourceLoader.Load<ActionRegistry>(ReadonlyPath("resources/action_registry.tres"));
    }
}
