using System.Collections.Generic;

public sealed record ActionDef(
    string Id,
    string Name,
    Targeting TargetRule,
    IReadOnlyList<EffectDef> Effects
);

public sealed class ComposedBattleAction(
    ActionDef def,
    Battler source,
    IReadOnlyList<Battler> targets,
    IReadOnlyList<IEffect> effects) : IBattleAction
{
    public Battler Source => source;
    public ActionDef Def => def;
    public IReadOnlyList<Battler> Targets => targets;

    private readonly IReadOnlyList<IEffect> _effects = effects;

    public void Execute(BattleModel model)
    {
        // effects may need more information to apply animations
        foreach (var e in _effects)
            e.Apply(model, source, targets);
    }
}
