using System.Threading;
using System.Threading.Tasks;

public sealed record DamageEffect(
    DamageType DamageType,
    DamageTypeMode DamageTypeMode,
    float Power,
    bool CanCrit
) : EffectDef
{
    public override async Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
    {
        var spec = new DamageSpec(DamageType, DamageTypeMode, Power, CanCrit);
        var calc = ctx.DamageRegistry.Get(spec.DmgType);

        foreach (var t in ctx.Targets)
        {
            ct.ThrowIfCancellationRequested();

            var amount = calc.Compute(ctx.Source, t, ctx.Model, spec);

            // Model changes (engine-agnostic)
            // t.Hp -= amount;
            // ctx.Model.Events.DamageDealt(ctx.Source, t, amount);

            ctx.Runtime.Log($"Apply damage: {amount}");
            await ctx.Runtime.ShowDamage(amount, ct);
        }
    }
}

public sealed record ApplyStatusEffect(
    string StatusId,
    int Turns
) : EffectDef
{
    public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        foreach (var t in ctx.Targets)
        {
            // Model changes (engine-agnostic)
            // t.Statuses.Add(StatusId, turns: Turns);

            ctx.Runtime.Log($"Apply status effect: {StatusId} ({Turns} turns)");
        }

        return Task.CompletedTask;
    }
}
