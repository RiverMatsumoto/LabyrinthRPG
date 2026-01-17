
public abstract class BattleState
{
    protected BattleScene Context { get; }

    public abstract void Enter();
    public abstract void Update(double delta);
    public abstract void Exit();

    protected void TransitionTo(BattleState newState) => Context.ChangeState(newState);
}

// Concrete States
public class ActionSelectionPhase : BattleState
{
    private BattleScene context;

    public ActionSelectionPhase()
    {
    }

    public ActionSelectionPhase(BattleScene context)
    {
        this.context = context;
    }

    public override void Enter()
    {
        // Show action menu for current battler
        // Enable target selection
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update(double delta)
    {
        // Handle player input for action/target selection
    }
}

public class TurnResolutionPhase : BattleState
{
    private TurnPlan _currentTurn;
    private int _actionIndex = 0;
    private BattleScene context;

    public TurnResolutionPhase(BattleScene context)
    {
        this.context = context;
    }

    public override void Enter()
    {
        // _currentTurn = GenerateTurnPlan();
        _actionIndex = 0;
        ProcessNextAction();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update(double delta)
    {
        throw new System.NotImplementedException();
    }

    private void ProcessNextAction()
    {
        if (_currentTurn.IsComplete)
        {
            TransitionTo(new ActionSelectionPhase());
            return;
        }

        var action = _currentTurn.GetNextAction();
        // ExecuteBattleAction(action);
    }
}
