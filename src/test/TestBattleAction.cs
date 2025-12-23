using System;
using System.IO;
using System.Collections.Generic;
using Godot;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;

public partial class TestBattleAction : Node
{

    private IActionLibrary actionLibrary;

    public override void _Ready()
    {
        actionLibrary = DI.Get<IActionLibrary>();
        ListBattleActions();
    }

    private void ListBattleActions()
    {
        GD.Print("Getting all actions...");
        IReadOnlyList<ActionDef> defs = actionLibrary.GetAll();
        foreach (var def in defs)
        {
            GD.Print(def.ToString());
        }

        // take the definitions and serialize them to see what comes out
        // Serialize defs to YAML file
        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();


        var wrapper = new Dictionary<string, IReadOnlyList<ActionDef>>
        {
            ["actions"] = defs
        };
        string yaml = serializer.Serialize(wrapper);

        string filePath = "actions_export.yaml";
        // Use System.IO.File instead of Godot's File class
        try
        {
            File.WriteAllText(filePath, yaml);
            GD.Print($"Serialized action defs saved to {filePath}");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to write file: {filePath}. Exception: {e.Message}");
        }
    }
}
