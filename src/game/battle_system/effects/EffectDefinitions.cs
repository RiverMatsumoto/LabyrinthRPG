using System.Threading;
using System.Threading.Tasks;
using Godot;

public abstract record EffectDef
{
    public abstract Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct);
}

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

            // Apply model changes here (HP, events, etc.)
            // t.Hp -= amount;
            // ctx.Model.Events.DamageDealt(ctx.Source, t, amount);
            //
            GD.Print($"Apply damage: {amount}");

            // currently only supports 1 damage pop up at a time
            await ctx.DamagePopup.ShowAsync(amount, ct);
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
        foreach (var t in ctx.Targets)
        {
            // t.Statuses.Add(StatusId, turns: Turns);
            GD.Print($"Apply status effect: {StatusId}");
        }
        return Task.CompletedTask;
    }
}
