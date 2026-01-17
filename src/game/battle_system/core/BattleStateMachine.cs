using System.Collections.Generic;
using Godot;

public partial class BattleStateMachine : Node
{
    [Export] private BattleScene context;
    private BattleState _currentState;
    private Dictionary<System.Type, BattleState> _states = new();

    public void Initialize()
    {
        _states.Add(typeof(ActionSelectionPhase), new ActionSelectionPhase(context));
        _states.Add(typeof(TurnResolutionPhase), new TurnResolutionPhase(context));

        ChangeState(new ActionSelectionPhase(context));
    }

    public void ChangeState(BattleState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
}
