using System.Collections.Generic;

public interface IEffect
{
    void Apply(BattleModel model, Battler source, IReadOnlyList<Battler> targets);
}

public interface IEffectBuilder
{
    string EffectName { get; }
    IEffect Build(EffectDef def);
}

public interface IEffectFactory
{
    IEffect Create(EffectDef def);
}

public interface IBattleAction
{
    Battler Source { get; }
    ActionDef Def { get; }
    IReadOnlyList<Battler> Targets { get; }
    void Execute(BattleModel model);
}

public interface IBattleActionFactory
{
    IBattleAction Create(ActionDef def, Battler source, IReadOnlyList<Battler> targets);
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
