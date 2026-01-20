using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;
using Godot;

public sealed record ActionDef(
    string Id,
    string Name,
    Targeting TargetRule,
    IList<IEffect> Effects
);

[GlobalClass]
public sealed partial class ActionRegistry : Resource
{
    [Export(PropertyHint.File, "*.yaml")]
    private string _actionsPath;
    private Dictionary<string, ActionDef> _defs;
    private readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    public void Load()
    {
        if (_defs != null) return;

        var yamlText = FileAccess.GetFileAsString(_actionsPath);
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
                                new DamageSpec(
                                    Enum.Parse<DamageType>((string)m["damage_type"], true),
                                    Enum.Parse<DamageTypeMode>((string)m["damage_type_mode"], true),
                                    Convert.ToSingle(m["power"]),
                                    Convert.ToSingle(m["crit_multiplier"]),
                                    Convert.ToBoolean(m["can_crit"])
                                )
                            ),
            "apply_status" => new ApplyStatusEffect(
                                Enum.Parse<Status>((string)m["status_effect"]),
                                Convert.ToInt32(m["stacks"])
                            ),
            "play_anim" => new PlayAnimEffect((string)m["anim_id"]),
            "play_anim_wait" => new PlayAnimWaitEffect((string)m["anim_id"]),
            "wait" => new WaitSecondsEffect(Convert.ToSingle(m["seconds"])),
            _ => throw new InvalidOperationException($"Unknown effect type: {type}"),
        };
    }

    public ActionDef Get(string id)
    {
        if (_defs == null) Load();
        return _defs.TryGetValue(id, out var def) ? def : throw new InvalidOperationException($"Unknown action id: {id}");
    }

    public IReadOnlyList<ActionDef> GetAll()
    {
        if (_defs == null) Load();
        return _defs.Values.ToList();
    }
}
