using System;
using System.Linq;
using Godot;

public abstract class BattleState
{
    /// A reference to the BattleScene. Can be used as a host for scene related behavior
    protected BattleScene Bs { get; }

    protected BattleState(BattleScene bs)
    {
        Bs = bs;
    }

    public abstract void Enter();
    public virtual void Update(double delta) { }
    public abstract void Exit();

    protected void TransitionTo(Type newState) => Bs.ChangeBattleState(newState);
}

/// <summary>
/// Manages the initialization phase of battle.
/// </summary>
public sealed class InitializeBattlePhase : BattleState
{
    public InitializeBattlePhase(BattleScene bs) : base(bs) { }

    public override void Enter()
    {
        Bs.InitializeUI();
        // later can do animations at the start of a battle then call ChangeBattleState once its done
        TransitionTo(typeof(ActionSelectionPhase));
    }

    public override void Exit()
    {
    }
}


/// <summary>
/// Manages the turn resolution phase of battle.
/// Executes all planned actions in priority order.
/// </summary>
public sealed class TurnResolutionPhase : BattleState
{
    private TurnPlan _currentTurn;

    public TurnResolutionPhase(BattleScene bs) : base(bs)
    {
        _currentTurn = bs.ctx.TurnPlan;
    }

    public override void Enter()
    {
        GD.Print("=== Turn Resolution Phase ===");

        if (_currentTurn.PlannedActions.Count == 0)
        {
            GD.PrintErr("No actions to execute!");
            TransitionTo(typeof(ActionSelectionPhase));
            return;
        }

        // Configure action executor
        Bs.actionExecutor.ActionFinished += OnActionFinished;

        // Start executing actions
        ExecuteNextAction();
    }

    public override void Exit()
    {
        Bs.actionExecutor.ActionFinished -= OnActionFinished;
    }

    private void ExecuteNextAction()
    {
        // Check for battle end conditions
        if (CheckBattleEndConditions())
            return;

        if (_currentTurn.IsComplete)
        {
            GD.Print("Turn complete! Starting new turn...");
            // Reset turn plan for next turn
            _currentTurn.ClearActions();
            _currentTurn.CurrentActorIndex = 0;

            // Go back to action selection
            TransitionTo(typeof(ActionSelectionPhase));
            return;
        }

        var battleAction = _currentTurn.GetNextAction();

        // Skip if source is dead
        if (!battleAction.Source.IsAlive)
        {
            GD.Print($"Skipping action from dead battler: {battleAction.Source.Stats.Name}");
            ExecuteNextAction();
            return;
        }

        GD.Print($"Executing: {battleAction.ActionDef.Name} by {battleAction.Source.Stats.Name}");

        // Update context with current action info
        Bs.ctx.Source = battleAction.Source;
        Bs.ctx.Targets = battleAction.Targets;

        // Configure and execute
        Bs.actionExecutor.Configure(Bs.ctx);
        Bs.actionExecutor.ExecutePlanned(battleAction);
    }

    private void OnActionFinished()
    {
        // Action complete, move to next
        ExecuteNextAction();
    }

    private bool CheckBattleEndConditions()
    {
        bool allEnemiesDead = !Bs.ctx.Model.enemyParty.Any(e => e.IsAlive);
        bool allPlayersDead = !Bs.ctx.Model.playerParty.Any(p => p.IsAlive);

        if (allEnemiesDead)
        {
            GD.Print("All enemies defeated!");
            TransitionTo(typeof(WinBattlePhase));
            return true;
        }

        if (allPlayersDead)
        {
            GD.Print("All players defeated!");
            TransitionTo(typeof(LoseBattlePhase));
            return true;
        }

        return false;
    }
}

/// <summary>
/// Manages the win battle phase of battle.
/// </summary>
public sealed class WinBattlePhase : BattleState
{
    public WinBattlePhase(BattleScene context) : base(context)
    {
    }

    public override void Enter()
    {
        GD.Print("=== VICTORY! ===");
        // TODO: Show victory screen, calculate rewards, grant exp, etc.
        // For now, just clean up
        Bs.CleanupBattle();
    }

    public override void Exit()
    {
    }
}

/// <summary>
/// Manages the lose battle phase of battle.
/// </summary>
public sealed class LoseBattlePhase : BattleState
{
    public LoseBattlePhase(BattleScene context) : base(context)
    {
    }

    public override void Enter()
    {
        GD.Print("=== DEFEAT ===");
        // TODO: Show game over screen, handle respawn/continue logic
        Bs.CleanupBattle();
    }

    public override void Exit()
    {
    }
}

/// <summary>
/// Manages the flee battle phase of battle.
/// </summary>
public sealed class FleeBattlePhase : BattleState
{
    public FleeBattlePhase(BattleScene context) : base(context)
    {
    }

    public override void Enter()
    {
        GD.Print("=== FLED BATTLE ===");
        // TODO: Handle flee logic, check if flee was successful
        Bs.CleanupBattle();
    }

    public override void Exit()
    {
    }
}
