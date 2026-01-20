using System.Collections.Generic;
using Godot;

public sealed class BattleModel
{
    public Party playerParty = new();
    public Party enemyParty = new();
}

/// <summary>
/// Represents the runtime environment for executing battle actions.
/// Contains all the context needed during battle execution.
/// </summary>
public sealed class BattleRunCtx
{
    public BattleModel Model { get; set; }
    public Battler Source { get; set; }
    public List<Battler> Targets { get; set; }
    public TurnPlan TurnPlan { get; set; }
    public Dictionary<Battler, Control> TargetNodes { get; set; }
    public DamageCalculatorRegistry DamageRegistry { get; }
    public RandomNumberGenerator Rng { get; set; }

    public BattleRunCtx(
        BattleModel model,
        TurnPlan turnPlan,
        Dictionary<Battler, Control> targetNodes,
        DamageCalculatorRegistry damageRegistry,
        RandomNumberGenerator rng)
    {
        Model = model;
        TurnPlan = turnPlan;
        TargetNodes = targetNodes;
        DamageRegistry = damageRegistry;
        Rng = rng;

        // Source and Targets are set dynamically during action execution
        Source = null;
        Targets = new List<Battler>();
    }
}

/// <summary>
/// Represents a single action to be executed during battle.
/// </summary>
public record BattleAction(
    ActionDef ActionDef,
    Battler Source,
    List<Battler> Targets,
    int Priority = 0,
    BattleActionType Type = BattleActionType.TurnBased
);

/// <summary>
/// Types of battle actions.
/// </summary>
public enum BattleActionType
{
    TurnBased,    // Player-selected actions
    Reaction,     // Counter-attacks, triggered abilities
    Environmental // Status effects, traps, field effects
}

/// <summary>
/// Manages the queue of planned actions for the current turn.
/// </summary>
public class TurnPlan
{
    public List<BattleAction> PlannedActions { get; } = new();
    public int CurrentActorIndex { get; set; } = 0;
    public bool IsComplete => CurrentActorIndex >= PlannedActions.Count;

    public void AddAction(BattleAction action) => PlannedActions.Add(action);
    public BattleAction GetNextAction() => PlannedActions[CurrentActorIndex++];
    public void ClearActions()
    {
        PlannedActions.Clear();
        CurrentActorIndex = 0;
    }
}
