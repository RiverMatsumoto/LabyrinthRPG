using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public sealed partial class DamageCalculatorRegistry : Resource, IDamageCalculatorRegistry
{
    public Dictionary<DamageType, IDamageCalculator> _map;

    public DamageCalculatorRegistry()
    {
        _map = new Dictionary<DamageType, IDamageCalculator>
        {
            { DamageType.Cut, new CutDamageCalculator() },
            { DamageType.Bash, new BashDamageCalculator() },
            { DamageType.Stab, new StabDamageCalculator() },
            { DamageType.Fire, new FireDamageCalculator() },
            { DamageType.Ice, new IceDamageCalculator() },
            { DamageType.Lightning, new LightningDamageCalculator() },
            { DamageType.True, new TrueDamageCalculator() }
        };
    }

    public IDamageCalculator Get(DamageType kind)
    {
        if (!_map.TryGetValue(kind, out var calc))
            throw new InvalidOperationException($"No calculator for {kind}");

        return calc;
    }
}
