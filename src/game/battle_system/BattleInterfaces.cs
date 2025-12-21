using System.Collections.Generic;

public interface IEffect
{
    void Apply(BattleModel model, Battler source, IReadOnlyList<Battler> targets);
}

public interface IEffectBuilder
{
    IEffect Build(EffectDef def);
}

public interface IBattleAction
{
    string Id { get; }
    void Execute(BattleModel model, Battler source, IReadOnlyList<Battler> targets);
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
