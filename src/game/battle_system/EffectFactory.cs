
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class EffectFactory : IEffectFactory
{
    private readonly Dictionary<string, IEffectBuilder> _builders;

    public EffectFactory(IEnumerable<IEffectBuilder> builders)
    {
        _builders = builders.ToDictionary(b => b.EffectName, b => b);
    }

    public IEffect Create(EffectDef def)
    {
        if (!_builders.TryGetValue(def.EffectName, out var builder))
            throw new InvalidOperationException($"Unknown effect: {def.EffectName}");

        return builder.Build(def);
    }
}
