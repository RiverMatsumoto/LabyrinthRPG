

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


public sealed record PlayAnimEffect(
    string AnimId
) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
    {
        ctx.Animator.Play(AnimId);
        return Task.CompletedTask;
    }
}


public sealed record PlayAnimWaitEffect(
    string AnimId
) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct) => ctx.Animator.PlayAsync(AnimId, ct);
}


public sealed record WaitEffect(
    int Ms
) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct) => Task.Delay(Ms, ct);
}
