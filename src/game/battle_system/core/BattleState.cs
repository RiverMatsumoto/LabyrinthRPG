using System;
using System.Collections.Generic;
using Godot;

public abstract class BattleState
{
    protected BattleScene Bs { get; }
    protected BattleRunCtx Ctx { get; }

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
        GD.Print("=== Battle System Test Start ===");
        var battleModel = new BattleModel();
        battleModel.playerParty.AddToFrontRow(new Battler());
        battleModel.playerParty.AddToFrontRow(new Battler());
        battleModel.playerParty.AddToBackRow(new Battler());
        var enemy1 = new Battler(Bs.enemyRegistry.GetEnemy("squid_wizard"));
        var enemy2 = new Battler(Bs.enemyRegistry.GetEnemy("squid_wizard"));
        battleModel.enemyParty.AddToFrontRow(enemy1);
        battleModel.enemyParty.AddToFrontRow(enemy2);
        Dictionary<Battler, Control> battlerUINodes = new();
        foreach (var member in battleModel.playerParty.GetFrontRowMembers())
        {
            var memberNode = Bs.AddPartyMemberFrontRow(member);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (var member in battleModel.playerParty.GetBackRowMembers())
        {
            var memberNode = Bs.AddPartyMemberBackRow(member);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (var enemy in battleModel.enemyParty)
        {
            var enemyNode = Bs.AddEnemyToBattle(enemy);
            battlerUINodes.Add(enemy, enemyNode);
        }

        // Fake battle context
        var source = battleModel.playerParty.GetFrontRowMember(0);
        var target = enemy1;

        var playback = new PlaybackOptions();

        Bs.ctx = new BattleRunCtx(
            model: battleModel,
            source: source,
            targets: new List<Battler> { target },
            turnPlan: new TurnPlan(),
            targetNodes: battlerUINodes,
            damageRegistry: Bs.damageCalculatorRegistry,
            rng: new RandomNumberGenerator()
        );

        // later can do animations at the start of a battle then call ChangeBattleState once its done

        Bs.ChangeBattleState(typeof(ActionSelectionPhase));
    }

    public override void Exit()
    {
    }
}

/// <summary>
/// Manages the action selection phase of battle.
/// </summary>
public sealed class ActionSelectionPhase : BattleState
{
    // private ActionSelectionUI actionSelectionUI;
    public ActionSelectionPhase(BattleScene bs) : base(bs)
    {
    }

    public override void Enter()
    {
        // Show action menu for current battler
        // Enable target selection from the context
    }

    public override void Exit()
    {

    }
}

/// <summary>
/// Manages the turn resolution phase of battle.
/// </summary>
public sealed class TurnResolutionPhase : BattleState
{
    private TurnPlan _currentTurn;
    private int _actionIndex = 0;

    public TurnResolutionPhase(BattleScene bs) : base(bs)
    {
        _currentTurn = bs.ctx.TurnPlan;
    }

    public override void Enter()
    {
        Bs.actionExecutor.Configure(Bs.ctx);
        Bs.actionExecutor.ActionFinished += Bs.OnActionFinished;

        Bs.InitializeUI();
    }

    public override void Exit()
    {

    }

    private void ProcessNextAction()
    {

        var action = _currentTurn.GetNextAction();
        // ExecuteBattleAction(action);
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
        // run win battle logic
    }

    public override void Exit()
    {

    }

}

public sealed class LoseBattlePhase : BattleState
{
    public LoseBattlePhase(BattleScene context) : base(context)
    {
    }

    public override void Enter()
    {
        // run lose battle logic
    }

    public override void Exit()
    {

    }

}

public sealed class FleeBattlePhase : BattleState
{
    public FleeBattlePhase(BattleScene context) : base(context)
    {
    }

    public override void Enter()
    {
        // run flee battle logic
    }

    public override void Exit()
    {

    }

}
