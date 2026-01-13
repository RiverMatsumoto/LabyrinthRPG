using System;

public static class DamageMath
{
    // Tunables (keep these centralized so you can rebalance easily)
    public const float PhysStrWeight = 1.00f;
    public const float PhysAtkWeight = 1.00f;
    public const float PhysDefWeight = 1.00f;
    public const float PhysVitWeight = 0.75f;

    public const float ElemTecWeight = 1.00f;
    public const float ElemAtkWeight = 0.50f;
    public const float ElemWisWeight = 1.35f; // higher emphasis on Wis
    public const float ElemDefWeight = 0.35f; // small contribution from general Def

    public const float CritMultiplier = 1.8f;

    public static int Physical(BattlerStats s, BattlerStats t, DamageSpec spec, float typeMult)
    {
        // "base" from spec + stats; you can swap spec.BasePower with whatever your DamageSpec uses
        float basePow = GetBasePower(spec);
        float offense =
            basePow +
            (s.Str * PhysStrWeight) +
            (s.Atk * PhysAtkWeight);

        float defense =
            (t.Def * PhysDefWeight) +
            (t.Vit * PhysVitWeight);

        // Smooth reduction that never goes negative or explodes: damage ~= offense * (off/(off+def))
        float dmg = offense * (offense / (offense + defense + 1f));

        dmg *= typeMult;
        dmg = ApplySpecMultipliers(dmg, spec);

        return Math.Max(0, (int)MathF.Round(dmg));
    }

    public static int Elemental(BattlerStats s, BattlerStats t, DamageSpec spec, float typeMult)
    {
        float basePow = GetBasePower(spec);
        float offense =
            basePow +
            (s.Tec * ElemTecWeight) +
            (s.Atk * ElemAtkWeight);

        // decreases as Wis and Tec increases, higher emphasis on Wis
        float resist =
            (t.Wis * ElemWisWeight) +
            (t.Tec * 0.65f) +
            (t.Def * ElemDefWeight);

        float dmg = offense * (offense / (offense + resist + 1f));

        dmg *= typeMult;
        dmg = ApplySpecMultipliers(dmg, spec);

        return Math.Max(0, (int)MathF.Round(dmg));
    }

    public static float ApplyCrit(BattleRunCtx ctx, BattlerStats s, BattlerStats t, DamageSpec spec, float dmg)
    {
        if (!spec.CanCrit) return dmg;

        // Simple Luc vs Luc curve
        float chance = 0.05f + (s.Luc - t.Luc) * 0.0025f; // 5% base, ±0.25% per Luc diff
        chance = Math.Clamp(chance, 0.01f, 0.50f);

        return (ctx.Rng.Randf() < chance) ? dmg * CritMultiplier : dmg;
    }

    // ---- helpers: adapt these to your DamageSpec fields ----

    private static float GetBasePower(DamageSpec spec)
    {
        // Replace with your real field(s)
        // e.g. spec.Power, spec.Amount, spec.Base, etc.
        return spec.Power;
    }

    private static float ApplySpecMultipliers(float dmg, DamageSpec spec)
    {
        // Replace with your real fields (crit, variance, skill multiplier, etc.)
        // Minimal default:
        return dmg * spec.Power;
    }
}

public sealed class CutDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Cut;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        int dmg = DamageMath.Physical(source.Stats, target.Stats, spec, typeMult: 1.00f);
        dmg = (int)DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}

public sealed class BashDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Bash;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        int dmg = DamageMath.Physical(source.Stats, target.Stats, spec, typeMult: 1.05f);
        dmg = (int)DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}

public sealed class StabDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Stab;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        int dmg = DamageMath.Physical(source.Stats, target.Stats, spec, typeMult: 0.95f);
        dmg = (int)DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}

public sealed class FireDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Fire;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        int dmg = DamageMath.Elemental(source.Stats, target.Stats, spec, typeMult: 1.00f);
        dmg = (int)DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}

public sealed class IceDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Ice;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        int dmg = DamageMath.Elemental(source.Stats, target.Stats, spec, typeMult: 0.98f);
        dmg = (int)DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}

public sealed class LightningDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.Lightning;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        int dmg = DamageMath.Elemental(source.Stats, target.Stats, spec, typeMult: 1.02f);
        dmg = (int)DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}

public sealed class TrueDamageCalculator : IDamageCalculator
{
    public DamageType Kind => DamageType.True;

    public int Compute(BattleRunCtx ctx, Battler source, Battler target, BattleModel model, DamageSpec spec)
    {
        var s = source.Stats;

        float basePow = spec.Power;
        float offense = basePow + s.Str + s.Tec + s.Atk;

        float dmg = offense; // true damage ignores defenses; spec.Power is already included as basePow

        dmg = DamageMath.ApplyCrit(ctx, source.Stats, target.Stats, spec, dmg);
        return Math.Max(0, (int)MathF.Round(dmg));
    }
}
