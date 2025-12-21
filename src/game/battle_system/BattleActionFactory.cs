using System.Collections.Generic;
using System.Linq;

public sealed class BattleActionFactory : IBattleActionFactory
{
    private readonly IEffectFactory _effects;

    public BattleActionFactory(IEffectFactory effects) => _effects = effects;

    public IBattleAction Create(ActionDef def, Battler source, IReadOnlyList<Battler> targets)
    {
        IReadOnlyList<IEffect> builtEffects = def.Effects.Select(ed => _effects.Create(ed)).ToList();
        return new ComposedBattleAction(def, source, targets, builtEffects);
    }


}
