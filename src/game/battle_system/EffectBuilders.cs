using System;
using System.Collections.Generic;

public sealed record EffectDef(
    string EffectName,
    Dictionary<string, object> Args
);

public sealed class DamageEffectBuilder(IDamageCalculatorRegistry reg) : IEffectBuilder
{
    public string EffectName => "damage";

    private readonly IDamageCalculatorRegistry _reg = reg;

    public IEffect Build(EffectDef def)
    {
        var dmgType = Enum.Parse<DamageType>((string)def.Args["damage_type"]);
        var dmgTypeMode = Enum.Parse<DamageTypeMode>((string)def.Args["damage_type_mode"]);
        var power = Convert.ToSingle(def.Args["power"]);
        var canCrit = (bool)def.Args["can_crit"];

        return new DamageEffect(_reg, new DamageSpec(dmgType, dmgTypeMode, power, canCrit));
    }
}
