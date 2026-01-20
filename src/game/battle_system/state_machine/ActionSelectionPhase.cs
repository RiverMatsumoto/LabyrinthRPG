using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Manages the action selection phase of battle.
/// Iterates through all player party members to select their actions and targets.
/// </summary>
public sealed class ActionSelectionPhase : BattleState
{
    private List<Battler> _playerBattlers;
    private int _currentBattlerIndex;
    private Battler _currentBattler;
    private ActionDef _selectedAction;

    private enum SelectionStep
    {
        SelectingAction,
        SelectingTarget
    }

    private SelectionStep _currentStep;

    public ActionSelectionPhase(BattleScene bs) : base(bs)
    {
        Bs.actionSelectionMenu.SelectedAction += OnActionSelected;
    }

    public override void Enter()
    {
        GD.Print("=== Action Selection Phase ===");

        // Get all alive player battlers
        _playerBattlers = Bs.ctx.Model.playerParty.Where(b => b.IsAlive).ToList();
        _currentBattlerIndex = 0;

        if (_playerBattlers.Count == 0)
        {
            GD.PrintErr("No alive player battlers!");
            TransitionTo(typeof(LoseBattlePhase));
            return;
        }

        // Start with first battler
        _currentBattler = _playerBattlers[_currentBattlerIndex];
        _currentStep = SelectionStep.SelectingAction;

        // Show action menu for current battler
        ShowActionMenuForCurrentBattler();
    }

    public override void Exit()
    {
        // Clean up UI state
        Bs.battleMenu?.Hide();
        Bs.targetSelectionUI?.HideTargetSelection();
    }

    private void ShowActionMenuForCurrentBattler()
    {
        GD.Print($"Selecting action for: {_currentBattler.Stats.Name}");

        // TODO: Show actual action menu UI
        // For now, we'll simulate selecting a basic attack
        // In a real implementation, you'd show a menu with Attack, Skill, Item, Defend, etc.

        // TEMPORARY: Auto-select a test action for demonstration
        // Replace this with actual UI menu logic
        // _selectedAction = Bs._actionRegistry.Get("FireAttack"); // or "BasicAttack"
        // OnActionSelected(_selectedAction);
        Bs.actionSelectionMenu.Show();
    }

    /// <summary>
    /// Called when an action is selected from the action menu.
    /// This would be called by your action menu UI.
    /// </summary>
    public void OnActionSelected(string actionId)
    {
        // resolve actionId
        Bs.actionSelectionMenu.Hide();
        var action = GameGlobals.Instance.ActionRegistry.Get(actionId);

        _selectedAction = action;
        _currentStep = SelectionStep.SelectingTarget;

        // Show target selection based on the action's targeting rule
        ShowTargetSelectionForAction(action);
    }

    private void ShowTargetSelectionForAction(ActionDef action)
    {
        GD.Print($"Selecting targets for: {action.Name}");

        // Show target selection UI
        Bs.targetSelectionUI.ShowTargetSelection(
            rule: action.TargetRule,
            source: _currentBattler,
            ctx: Bs.ctx
        );

        // Connect to target selection completion (using method references, not signals)
        // The TargetSelectionUI will call these callbacks
        ConnectTargetSelectionCallbacks();
    }

    private void ConnectTargetSelectionCallbacks()
    {
        // Connect signals for target selection
        Bs.targetSelectionUI.TargetSelected += OnTargetSelected;
        Bs.targetSelectionUI.TargetSelectionCancelled += OnTargetSelectionCancelled;
    }

    private void DisconnectTargetSelectionCallbacks()
    {
        // Disconnect signals
        if (Bs.targetSelectionUI != null)
        {
            Bs.targetSelectionUI.TargetSelected -= OnTargetSelected;
            Bs.targetSelectionUI.TargetSelectionCancelled -= OnTargetSelectionCancelled;
        }
    }

    /// <summary>
    /// Called when a target is selected.
    /// </summary>
    private void OnTargetSelected(Battler target)
    {
        DisconnectTargetSelectionCallbacks();

        GD.Print($"Target selected: {target.Stats.Name}");

        // Get all targets based on targeting rule
        List<Battler> targets = GetFinalTargets(_selectedAction.TargetRule, target);

        // Create battle action and add to turn plan
        var battleAction = new BattleAction(
            ActionDef: _selectedAction,
            Source: _currentBattler,
            Targets: targets,
            Priority: CalculatePriority(_currentBattler, _selectedAction)
        );

        Bs.ctx.TurnPlan.AddAction(battleAction);

        GD.Print($"Added action to turn plan: {_selectedAction.Name} by {_currentBattler.Stats.Name}");

        // Move to next battler
        AdvanceToNextBattler();
    }

    /// <summary>
    /// Called when target selection is cancelled.
    /// </summary>
    private void OnTargetSelectionCancelled()
    {
        DisconnectTargetSelectionCallbacks();

        GD.Print("Target selection cancelled, returning to action menu");

        // Go back to action selection
        _currentStep = SelectionStep.SelectingAction;
        _selectedAction = null;
        ShowActionMenuForCurrentBattler();
    }

    /// <summary>
    /// Gets all final targets based on the targeting rule and selected target.
    /// For single-target rules, returns just the selected target.
    /// For multi-target rules, returns all valid targets in the group.
    /// </summary>
    private List<Battler> GetFinalTargets(Targeting rule, Battler selectedTarget)
    {
        return rule switch
        {
            Targeting.SingleEnemy => new List<Battler> { selectedTarget },
            Targeting.SingleAlly => new List<Battler> { selectedTarget },
            Targeting.Self => new List<Battler> { _currentBattler },
            Targeting.AllEnemies => Bs.ctx.Model.enemyParty.Where(b => b.IsAlive).ToList(),
            Targeting.AllAllies => Bs.ctx.Model.playerParty.Where(b => b.IsAlive).ToList(),
            // TODO: Implement row-based targeting
            // Targeting.RowEnemies => GetEnemiesInSameRow(selectedTarget),
            // Targeting.RowAllies => GetAlliesInSameRow(selectedTarget),
            _ => new List<Battler> { selectedTarget }
        };
    }

    /// <summary>
    /// Calculates action priority for turn order.
    /// Higher priority = acts first.
    /// </summary>
    private int CalculatePriority(Battler battler, ActionDef action)
    {
        // Base priority on battler speed/agility
        // For now, return a simple value
        // TODO: Factor in battler stats, action type, etc.
        return battler.Stats.Agi;
    }

    /// <summary>
    /// Advances to the next battler in the party.
    /// If all battlers have selected actions, also plan enemy actions and transition to resolution.
    /// </summary>
    private void AdvanceToNextBattler()
    {
        _currentBattlerIndex++;

        if (_currentBattlerIndex >= _playerBattlers.Count)
        {
            // All player battlers have selected actions
            GD.Print("All player actions selected!");

            // Plan enemy actions
            PlanEnemyActions();

            // Sort actions by priority
            SortTurnPlan();

            // Transition to turn resolution
            TransitionTo(typeof(TurnResolutionPhase));
        }
        else
        {
            // Move to next battler
            _currentBattler = _playerBattlers[_currentBattlerIndex];
            _currentStep = SelectionStep.SelectingAction;
            _selectedAction = null;
            ShowActionMenuForCurrentBattler();
        }
    }

    /// <summary>
    /// Plans actions for all enemy battlers using AI logic.
    /// </summary>
    private void PlanEnemyActions()
    {
        GD.Print("Planning enemy actions...");

        foreach (var enemy in Bs.ctx.Model.enemyParty.Where(e => e.IsAlive))
        {
            // Simple AI: Pick a random action and target
            // TODO: Implement proper AI logic
            var action = SelectEnemyAction(enemy);
            var targets = SelectEnemyTargets(enemy, action);

            var battleAction = new BattleAction(
                ActionDef: action,
                Source: enemy,
                Targets: targets,
                Priority: CalculatePriority(enemy, action)
            );

            Bs.ctx.TurnPlan.AddAction(battleAction);
            GD.Print($"Enemy action planned: {action.Name} by {enemy.Stats.Name}");
        }
    }

    /// <summary>
    /// Selects an action for an enemy using AI logic.
    /// </summary>
    private ActionDef SelectEnemyAction(Battler enemy)
    {
        // TODO: Implement proper AI action selection
        // For now, just return a basic attack or random skill
        return Bs._actionRegistry.Get("BasicAttack");
    }

    /// <summary>
    /// Selects targets for an enemy action.
    /// </summary>
    private List<Battler> SelectEnemyTargets(Battler enemy, ActionDef action)
    {
        // Select targets based on action's targeting rule
        var validTargets = TargetSelector.GetValidTargets(action.TargetRule, enemy, Bs.ctx.Model, sourceIsEnemy: true);

        if (validTargets.Count == 0)
            return new List<Battler>();

        // For single target, pick randomly (or use AI logic)
        if (action.TargetRule == Targeting.SingleAlly || action.TargetRule == Targeting.SingleEnemy)
        {
            int randomIndex = Bs.ctx.Rng.RandiRange(0, validTargets.Count - 1);
            return new List<Battler> { validTargets[randomIndex] };
        }

        // For multi-target, return all valid targets
        return validTargets;
    }

    /// <summary>
    /// Sorts the turn plan by priority (highest priority acts first).
    /// </summary>
    private void SortTurnPlan()
    {
        Bs.ctx.TurnPlan.PlannedActions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        GD.Print($"Turn order determined! {Bs.ctx.TurnPlan.PlannedActions.Count} actions planned.");
    }
}
