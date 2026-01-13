using System.Collections.Generic;
using Godot;


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
    int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec);
}

public interface IDamageCalculatorRegistry
{
    IDamageCalculator Get(DamageType kind);
}
