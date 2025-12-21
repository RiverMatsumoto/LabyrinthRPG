using System.Collections.Generic;

public sealed record ActionDef(
    string ActionName,
    Dictionary<string, object> Args
);

public sealed class ComposedBattleAction(string id, IReadOnlyList<IEffect> effects) : IBattleAction
{
    public string Id { get; } = id;
    private readonly IReadOnlyList<IEffect> _effects = effects;

    public void Execute(BattleModel model, Battler source, IReadOnlyList<Battler> targets)
    {
        // effects may need more information to apply animations
        foreach (var e in _effects)
            e.Apply(model, source, targets);
    }
}
