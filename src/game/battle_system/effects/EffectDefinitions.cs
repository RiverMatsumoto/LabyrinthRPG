using Godot;

public sealed record DamageEffect(
    DamageSpec DamageSpec
) : IEffect
{
    public IEffectWait Execute(BattleRunCtx ctx)
    {
        var spec = DamageSpec;
        foreach (var target in ctx.Targets)
        {
            var calc = ctx.DamageRegistry.Get(spec.DamageType);
            int damage = calc.Compute(
                ctx,
                ctx.Source,
                target,
                ctx.Model, new DamageSpec(
                spec.DamageType,
                spec.DamageTypeMode,
                spec.Power,
                spec.CritMultiplier,
                spec.CanCrit));
            GD.Print($"Apply damage effect: {damage}");
            target.Stats.Hp -= damage;
        }
        return new NoWait();
    }

}

public sealed record ApplyStatusEffect(
    Status Status,
    int Stacks
) : IEffect
{
    public IEffectWait Execute(BattleRunCtx ctx)
    {
        throw new System.NotImplementedException();
    }

    // public override Task ExecuteAsync(BattleRunCtx ctx, CancellationToken ct)
    // {
    //     ct.ThrowIfCancellationRequested();

    //     foreach (var t in ctx.Targets)
    //     {
    //         // Model changes (engine-agnostic)
    //         // t.Statuses.Add(StatusId, turns: Turns);

    //         ctx.Runtime.Log($"Apply status effect: {StatusId} ({Turns} turns)");
    //     }

    //     return Task.CompletedTask;
    // }
}
