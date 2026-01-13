using System;
using System.Collections.Generic;
using Godot;

public sealed partial class DamageCalculatorRegistry : Resource, IDamageCalculatorRegistry
{
    public Dictionary<DamageType, IDamageCalculator> _map;

    public DamageCalculatorRegistry()
    {
        _map = new Dictionary<DamageType, IDamageCalculator>();
        _map.Add(DamageType.Cut, new CutDamageCalculator());
        _map.Add(DamageType.Bash, new BashDamageCalculator());
        _map.Add(DamageType.Stab, new StabDamageCalculator());

        _map.Add(DamageType.Fire, new FireDamageCalculator());
        _map.Add(DamageType.Ice, new IceDamageCalculator());
        _map.Add(DamageType.Lightning, new LightningDamageCalculator());

        _map.Add(DamageType.True, new TrueDamageCalculator());
    }

    public IDamageCalculator Get(DamageType kind)
    {
        if (!_map.TryGetValue(kind, out var calc))
            throw new InvalidOperationException($"No calculator for {kind}");

        return calc;
    }
}
