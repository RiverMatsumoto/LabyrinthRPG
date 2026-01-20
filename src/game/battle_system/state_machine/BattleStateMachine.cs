using System;
using System.Collections.Generic;

/// <summary>
/// Manages the state transitions and execution of the battle.
/// All BattleStates:
///     InitializeBattlePhase
///     ActionSelectionPhase
///     TurnResolutionPhase
///     WinBattlePhase
///     LoseBattlePhase
/// </summary>
public sealed class BattleStateMachine
{
    private BattleScene _context;
    private BattleState _currentState;
    private Dictionary<Type, BattleState> _states = new();
    // add results of the statemachine that basically "exit" after the win, lose, or flee

    public BattleStateMachine(BattleScene context)
    {
        _context = context;
        _states.Add(typeof(InitializeBattlePhase), new InitializeBattlePhase(_context));
        _states.Add(typeof(ActionSelectionPhase), new ActionSelectionPhase(_context));
        _states.Add(typeof(TurnResolutionPhase), new TurnResolutionPhase(_context));

        // exit states. These states must deal with exiting battle cleanly
        _states.Add(typeof(WinBattlePhase), new WinBattlePhase(_context));
        _states.Add(typeof(LoseBattlePhase), new LoseBattlePhase(_context));
        _states.Add(typeof(FleeBattlePhase), new FleeBattlePhase(_context));
    }

    public void Initialize()
    {
        ChangeState(typeof(InitializeBattlePhase));
    }

    public void ChangeState(Type newState)
    {
        _currentState?.Exit();
        _currentState = _states[newState];
        _currentState.Enter();
    }
}
