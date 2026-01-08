using System.Collections.Generic;


public interface IEffectBuilder
{
    string EffectName { get; }
    IEffect Build(EffectDef def);
}

public interface IEffectFactory
{
    IEffect Create(EffectDef def);
}

public interface IDamageCalculator
{
    DamageType Kind { get; }
    int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec);
}

public interface IDamageCalculatorRegistry
{
    IDamageCalculator Get(DamageType kind);
}
