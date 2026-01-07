using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class BattleModel
{
    public Party playerParty = new();
    public Party enemyParty = new();

    public BattleModel() { }

    public void ResolveTurn() { }
}

public sealed class BattleRunCtx(
    BattleModel model,
    Battler source,
    IReadOnlyList<Battler> targets,
    IDamageCalculatorRegistry damageRegistry,
    IAnimator animator,
    IDamagePopup damagePopup)
{
    public BattleModel Model { get; } = model;
    public Battler Source { get; } = source;
    public IReadOnlyList<Battler> Targets { get; } = targets;

    public IDamageCalculatorRegistry DamageRegistry { get; } = damageRegistry;
    public IAnimator Animator { get; } = animator;
    public IDamagePopup DamagePopup { get; } = damagePopup;
}

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
