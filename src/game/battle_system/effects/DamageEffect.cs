using System.Collections.Generic;
using Godot;

public sealed class DamageEffect(IDamageCalculatorRegistry reg, DamageSpec dmgSpec) : IEffect
{
    private readonly IDamageCalculatorRegistry _reg = reg;
    private readonly DamageSpec _dmgSpec = dmgSpec;

    public void Apply(BattleModel model, Battler source, IReadOnlyList<Battler> targets)
    {
        var calc = _reg.Get(_dmgSpec.type);
        foreach (var t in targets)
        {
            var amount = calc.Compute(source, t, model, _dmgSpec);
            GD.Print($"Damage effect: {amount}, Source: {source.Stats.Name}, Target: {t}");
            // t.Hp -= amount;
            // model.Events.DamageDealt(source, t, amount);
        }
    }
}

public enum DamageType
{
    Cut,
    Bash,
    Stab,
    Fire,
    Ice,
    Lightning,
    True
}

public enum DamageTypeMode
{
    Fixed,
    FromWeapon
}

public sealed record DamageSpec(
    DamageType type = DamageType.True,
    DamageTypeMode dmgTypeMode = DamageTypeMode.FromWeapon,
    float Power = 1.0f,
    bool CanCrit = true
);
