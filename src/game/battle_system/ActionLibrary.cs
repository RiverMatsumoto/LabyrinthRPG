using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;
using Godot;

public interface IActionLibrary
{
    ActionDef Get(string id);
    IReadOnlyList<ActionDef> GetAll();
}

public sealed class ActionLibrary : IActionLibrary
{
    private readonly Dictionary<string, ActionDef> _defs;
    private IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    public ActionLibrary()
    {
        var yamlText = File.ReadAllText("src/game/battle_system/actions.yaml");
        var root = deserializer.Deserialize<Dictionary<string, object>>(yamlText);
        _defs = ParseActions(root);
    }

    private static Dictionary<string, ActionDef> ParseActions(Dictionary<string, object> root)
    {
        var result = new Dictionary<string, ActionDef>();

        var actions = (List<object>)root["actions"];

        foreach (var actionObj in actions)
        {
            var actionMap = (Dictionary<object, object>)actionObj;

            var id = (string)actionMap["id"];
            var name = (string)actionMap["name"];

            var effectsYaml = (List<object>)actionMap["effects"];
            var effects = new List<EffectDef>();

            foreach (var effectObj in effectsYaml)
            {
                var effectMap = (Dictionary<object, object>)effectObj;

                var effectName = (string)effectMap["type"];
                var args = new Dictionary<string, object>();

                foreach (var kv in effectMap)
                {
                    var key = (string)kv.Key;
                    if (key == "type")
                        continue;

                    // normalize numeric scalars
                    args[key] = kv.Value switch
                    {
                        int i => i,
                        long l => l,
                        double d => (float)d,
                        _ => kv.Value
                    };
                }

                effects.Add(new EffectDef(effectName, args));
            }

            result[id] = new ActionDef(
                Id: id,
                Name: name,
                TargetRule: Targeting.SingleEnemy, // example
                Effects: effects
            );
        }

        return result;
    }

    public ActionDef Get(string id) => _defs.TryGetValue(id, out var def) ? def : throw new InvalidOperationException($"Unknown action id: {id}");

    public IReadOnlyList<ActionDef> GetAll() => _defs.Values.ToList();
}
