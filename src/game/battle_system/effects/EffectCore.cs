using System.Threading;
using System.Threading.Tasks;
using Godot;

public interface IEffect
{
    IEffectWait Execute(BattleRunCtx ctx);
}

// Utility for timing animations and damage popup
public interface IEffectWait { }
public sealed record NoWait() : IEffectWait; // sentinel wait object
public sealed record WaitSeconds(
    float Seconds
) : IEffectWait;
public sealed record PlayDamagePopup(
    int Amount,
    bool Wait
) : IEffectWait;
public sealed record PlayAnim(
    string AnimId,
    bool Wait
) : IEffectWait;

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
    DamageType DamageType,
    DamageTypeMode DamageTypeMode,
    float Power,
    float CritMultiplier,
    bool CanCrit
);

public enum Status
{
    None,
    Confusion,
    Stun,
    BindHead,
    BindArm,
    BindLegs,
    Poison,
    Burn,
    Blind,
    Paralysis,
    Petrify,
    InstantDeath,
    Curse,
    Sleep,
}

public class PlaybackOptions
{
    public bool SkipWaits { get; set; } = false;
    public float Speed { get; set; } = 1.0f; // 1 = normal, 2 = 2x, etc.
}


public abstract record EffectDef
{
    public abstract Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct);
}

public sealed record WaitSecondsEffect(float Seconds) : IEffect
{
    public IEffectWait Execute(BattleRunCtx ctx) => new WaitSeconds(Seconds);
}

public sealed record PlayAnimEffect(string AnimId) : IEffect
{
    public IEffectWait Execute(BattleRunCtx ctx) => new PlayAnim(AnimId, Wait: false);
}

public sealed record PlayAnimWaitEffect(string AnimId) : IEffect
{
    public IEffectWait Execute(BattleRunCtx ctx) => new PlayAnim(AnimId, Wait: true);
}

public sealed record WaitDamagePopupEffect(int Amount) : IEffect
{
    public IEffectWait Execute(BattleRunCtx ctx) => new PlayDamagePopup(Amount, Wait: true);
}
