using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

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
        List<Battler> targets,
        Dictionary<Battler, Control> targetNodes,
        IDamageCalculatorRegistry damageRegistry,
        RandomNumberGenerator rng)
{
    public BattleModel Model { get; } = model;
    public Battler Source { get; } = source;
    public List<Battler> Targets { get; } = targets;
    public Dictionary<Battler, Control> TargetNodes { get; } = targetNodes;
    public IDamageCalculatorRegistry DamageRegistry { get; } = damageRegistry;
    public RandomNumberGenerator Rng { get; } = rng;
}

/// <summary>
/// Represents the runtime environment for executing effects.
/// </summary>
// public static class ActionExecutor
// {
//     public static async Task ExecuteAsync(
//         ActionDef action,
//         BattleRunCtx ctx,
//         CancellationToken ct = default)
//     {
//         foreach (var eff in action.Effects)
//             await eff.ExecuteAsync(ctx, ct);
//     }
// }

// public interface IActionExecutor
// {
//     void Execute(ActionDef action, BattleRunCtx ctx);
//     void SkipNow();
// }

/// <summary>
/// Represents the runtime environment for executing effects.
/// </summary>
public interface IEffectRuntime
{
    PlaybackOptions Playback { get; }
    IEffectWait WaitSeconds(float seconds);
    IEffectWait PlayAnim(string id, bool wait);
    IEffectWait ShowDamage(int amount);
    void Log(string msg);
}
