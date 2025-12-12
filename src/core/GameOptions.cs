using Godot;
using System;
using System.Collections.Generic;

public partial class GameOptions : Node
{
    public Dictionary<string, Variant> GameOptionsDict;
    public void LoadGameOptions()
    {
        ConfigFile config = new ConfigFile();
        Error err = config.Load("res://data/settings.cfg");

        if (err != Error.Ok)
        {
            GD.PushError($"Failed to load settings.cfg: {err}");
            return;
        }

        var sections = config.GetSections();
        foreach (var section in sections)
        {
            GameOptionsDict["fps"] = config.GetValue(section, "fps");
            GameOptionsDict["resolution"] = config.GetValue(section, "resolution");
        }
    }

    public void SaveGameOptions()
    {
        ConfigFile config = new ConfigFile();
        foreach (var key in GameOptionsDict.Keys)
        {
            config.SetValue("settings", key, GameOptionsDict[key]);
        }
        config.Save("res://data/settings.cfg");
    }
}
