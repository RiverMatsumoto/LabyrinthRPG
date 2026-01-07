using System.Threading;
using System.Threading.Tasks;

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
    DamageType DmgType,
    DamageTypeMode DmgTypeMode,
    float Power,
    bool CanCrit
);

public class PlaybackOptions
{
    bool SkipWaits { get; set; } = false;
    float Speed { get; set; } = 1.0f; // 1 = normal, 2 = 2x, etc.
}


public abstract record EffectDef
{
    public abstract Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct);
}

public sealed record WaitEffect(int Ms) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
        => ctx.Runtime.WaitMs(Ms, ct);
}

public sealed record PlayAnimEffect(string AnimId) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
    {
        ctx.Runtime.PlayAnim(AnimId, wait: true, ct);
        return Task.CompletedTask;
    }
}

public sealed record PlayAnimWaitEffect(string AnimId) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
        => ctx.Runtime.PlayAnim(AnimId, wait: true, ct);
}
