using System;
using System.Collections.Generic;

public sealed record EffectDef(
    string EffectName,
    Dictionary<string, object> Args
);

public sealed class DamageEffectBuilder(IDamageCalculatorRegistry reg) : IEffectBuilder
{
    private readonly IDamageCalculatorRegistry _reg = reg;

    public IEffect Build(EffectDef def)
    {
        var kind = Enum.Parse<DamageType>((string)def.Args["kind"]);
        var power = Convert.ToSingle(def.Args["power"]);

        return new DamageEffect(_reg, new DamageSpec(kind, power));
    }
}
