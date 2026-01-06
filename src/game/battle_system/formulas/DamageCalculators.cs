public sealed class CutDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Cut;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class BashDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Bash;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class StabDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Stab;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class FireDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Fire;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        throw new System.NotImplementedException();
    }
}
public sealed class IceDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Ice;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class LightningDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Lightning;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class TrueDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.True;

    public int Compute(Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        // temporary take str + atk
        var s = source.Stats;
        return s.Str + s.Atk;
    }
}
