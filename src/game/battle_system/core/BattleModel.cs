using System.Collections.Generic;
using Godot;

public sealed class BattleModel
{
    public Party playerParty = new();
    public Party enemyParty = new();
}

/// <summary>
/// Represents the runtime environment for executing effects.
/// </summary>
public sealed class BattleRunCtx(
    BattleModel model,
    Battler source,
    List<Battler> targets,
    TurnPlan turnPlan,
    Dictionary<Battler, Control> targetNodes,
    DamageCalculatorRegistry damageRegistry,
    RandomNumberGenerator rng)
{
    public BattleModel Model { get; set; } = model;
    public Battler Source { get; set; } = source;
    public List<Battler> Targets { get; set; } = targets;
    public TurnPlan TurnPlan { get; set; } = turnPlan;
    public Dictionary<Battler, Control> TargetNodes { get; set; } = targetNodes;
    public DamageCalculatorRegistry DamageRegistry { get; } = damageRegistry;
    public RandomNumberGenerator Rng { get; set; } = rng;
}

public record BattleAction(
    ActionDef ActionDef,
    Battler Source,
    List<Battler> Targets,
    int Priority = 0,
    BattleActionType Type = BattleActionType.TurnBased
);

// use reaction later for reaction based skills
// use environment later for status effects or multipliers
public enum BattleActionType
{
    TurnBased,    // Player selects
    Reaction,     // Counter-attacks, etc.
    Environmental // Status effects, traps, etc.
}

public class TurnPlan
{
    public List<BattleAction> PlannedActions { get; } = new();
    public int CurrentActorIndex { get; set; } = 0;
    public bool IsComplete => CurrentActorIndex >= PlannedActions.Count;

    public void AddAction(BattleAction action) => PlannedActions.Add(action);
    public BattleAction GetNextAction() => PlannedActions[CurrentActorIndex++];
}

// /// <summary>
// /// Represents the runtime environment for executing effects.
// /// </summary>
// public interface IEffectRuntime
// {
//     PlaybackOptions Playback { get; }
//     IEffectWait WaitSeconds(float seconds);
//     IEffectWait PlayAnim(string id, bool wait);
//     IEffectWait ShowDamage(int amount);
//     void Log(string msg);
// }
