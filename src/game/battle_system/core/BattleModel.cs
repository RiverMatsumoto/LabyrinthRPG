using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class BattleModel
{
    public Party playerParty = new();
    public Party enemyParty = new();

    public BattleModel()
    {

    }

    public void ResolveTurn() { }
}

/// <summary>
/// Represents the runtime environment for executing effects.
/// </summary>
public sealed class BattleRunCtx(
        BattleModel model,
        Battler source,
        IReadOnlyList<Battler> targets,
        IDamageCalculatorRegistry damageRegistry,
        IEffectRuntime runtime
    )
{
    public BattleModel Model { get; } = model;
    public Battler Source { get; } = source;
    public IReadOnlyList<Battler> Targets { get; } = targets;
    public IDamageCalculatorRegistry DamageRegistry { get; } = damageRegistry;
    public IEffectRuntime Runtime { get; } = runtime;
}

/// <summary>
/// Represents the runtime environment for executing effects.
/// </summary>
public static class ActionExecutor
{
    public static async Task ExecuteAsync(
        ActionDef action,
        BattleRunCtx ctx,
        CancellationToken ct = default)
    {
        foreach (var eff in action.Effects)
            await eff.ExecuteAsync(ctx, ct);
    }
}

/// <summary>
/// Represents the runtime environment for executing effects.
/// </summary>
public interface IEffectRuntime
{
    PlaybackOptions Playback { get; }
    Task WaitMs(int ms, CancellationToken ct);
    Task PlayAnim(string id, bool wait, CancellationToken ct);
    Task ShowDamage(int amount, CancellationToken ct);
    void Log(string msg);
}
