using System;
using System.Collections.Generic;
using System.Linq;

public sealed class DamageCalculatorRegistry : IDamageCalculatorRegistry
{
    private readonly Dictionary<DamageType, IDamageCalculator> _map;

    public DamageCalculatorRegistry(IEnumerable<IDamageCalculator> calculators)
    {
        _map = calculators.ToDictionary(c => c.Kind, c => c);
    }

    public IDamageCalculator Get(DamageType kind)
    {
        if (!_map.TryGetValue(kind, out var calc))
            throw new InvalidOperationException($"No calculator for {kind}");

        return calc;
    }
}
