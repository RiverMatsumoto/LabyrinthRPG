using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;

public sealed record ActionDef(
    string Id,
    string Name,
    Targeting TargetRule,
    IList<IEffect> Effects
);

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
            var targetRule = Enum.Parse<Targeting>((string)actionMap["target_rule"], true);

            var effectsYaml = (List<object>)actionMap["effects"];
            var effects = new List<IEffect>();

            foreach (var effectObj in effectsYaml)
            {
                var effectMap = (Dictionary<object, object>)effectObj;
                effects.Add(ParseEffect(effectMap));
            }

            result[id] = new ActionDef(
                Id: id,
                Name: name,
                TargetRule: targetRule, // example
                Effects: effects
            );
        }

        return result;
    }

    private static IEffect ParseEffect(Dictionary<object, object> m)
    {
        var type = (string)m["type"];

        return type switch
        {
            "damage" => new DamageEffect(
                                Enum.Parse<DamageType>((string)m["damage_type"], true),
                                Enum.Parse<DamageTypeMode>((string)m["damage_type_mode"], true),
                                Convert.ToSingle(m["power"]),
                                Convert.ToBoolean(m["can_crit"])
                            ),
            "apply_status" => new ApplyStatusEffect(
                                (string)m["status_id"],
                                Convert.ToInt32(m["stacks"])
                            ),
            "play_anim" => new PlayAnimEffect((string)m["anim_id"]),
            "play_anim_wait" => new PlayAnimWaitEffect((string)m["anim_id"]),
            "wait" => new WaitSecondsEffect(Convert.ToSingle(m["seconds"])),
            _ => throw new InvalidOperationException($"Unknown effect type: {type}"),
        };
    }

    public ActionDef Get(string id) => _defs.TryGetValue(id, out var def) ? def : throw new InvalidOperationException($"Unknown action id: {id}");

    public IReadOnlyList<ActionDef> GetAll() => _defs.Values.ToList();
}
