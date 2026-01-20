using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface IActionExecutor
{
    void Execute(ActionDef action);
    void SkipNow();
}

public sealed partial class GodotActionExecutor : Node, IActionExecutor
{
    [Signal] public delegate void ActionFinishedEventHandler();
    [Export] private ActionAnimator actionAnimator;
    [Export] private GameData gameData;
    private DamagePopup _currentPopup;
    private SceneTreeTimer _activeTimer;
    private BattleRunCtx _ctx;
    private ActionDef _currentAction = default!;
    private IEnumerator<IEffect> effectIterator;

    private enum ExecutionState { Idle, WaitingTimer, WaitingAnim, WaitingPopup }
    private ExecutionState _currentState = ExecutionState.Idle;
    private AnimatedSprite2D _currentSprite;

    public override void _Ready()
    {
    }

    public void Configure(BattleRunCtx ctx)
    {
        _ctx = ctx;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // SkipNow() on ui_accept
        if (Input.IsActionJustPressed("ui_accept"))
        {
            SkipNow();
        }
    }

    public void Execute(ActionDef action)
    {
        _currentAction = action;
        effectIterator = _currentAction.Effects.GetEnumerator();

        ExecuteNextEffect();
    }

    public void SkipNow()
    {
        switch (_currentState)
        {
            case ExecutionState.WaitingTimer:
                if (_activeTimer != null) _activeTimer.Timeout -= ExecuteNextEffect;
                break;
            case ExecutionState.WaitingAnim:
                if (_currentSprite != null) _currentSprite.AnimationFinished -= OnAnimationFinished;
                actionAnimator.CleanupAnimation(_currentSprite);
                break;
            case ExecutionState.WaitingPopup:
                _currentPopup.EndDamagePopup();
                _currentPopup.Finished -= OnDamagePopupFinished;
                break;
        }

        _currentState = ExecutionState.Idle;
        ExecuteNextEffect();
    }

    private void ExecuteNextEffect()
    {
        _currentState = ExecutionState.Idle;

        while (effectIterator.MoveNext())
        {
            var eff = effectIterator.Current; // easiest is: make ActionDef.Effects a List<IEffect>
            var wait = eff.Execute(_ctx);
            GD.Print($"Executing Effect: {eff}");

            switch (wait)
            {
                case NoWait:
                    continue;

                case WaitSeconds w:
                    _currentState = ExecutionState.WaitingTimer;
                    _activeTimer = GetTree().CreateTimer(w.Seconds * gameData.PlaybackOptions.Speed);
                    _activeTimer.Timeout += OnTimerFinished;
                    return;

                case PlayAnim anim:
                    _currentState = ExecutionState.WaitingAnim;
                    Vector2 position = _ctx.TargetNodes[_ctx.Targets.First()]
                        .GetNode<TextureRect>("TextureRect")
                        .GetGlobalRect()
                        .GetCenter();
                    _currentSprite = actionAnimator.PlayOnce(anim.AnimId, position, gameData.PlaybackOptions.Speed);
                    _currentSprite.AnimationFinished += OnAnimationFinished;
                    return;

                case PlayDamagePopup popup:
                    Vector2 damagePosition = _ctx.TargetNodes[_ctx.Targets.First()]
                        .GetNode<TextureRect>("TextureRect")
                        .GetGlobalRect()
                        .GetCenter();
                    _currentPopup = actionAnimator.PlayDamagePopup(
                        popup.Amount,
                        damagePosition,
                        gameData.PlaybackOptions.Speed);
                    if (popup.Wait)
                    {
                        _currentState = ExecutionState.WaitingPopup;
                        _currentPopup.FinishedDamagePopup += OnDamagePopupFinished;
                        return;
                    }
                    else
                    {
                        _currentState = ExecutionState.Idle;
                        continue;
                    }

                // same as no wait, but something went wrong likely if this runs
                default:
                    GD.PushError($"Unknown wait type: {wait.GetType().Name}");
                    continue;
            }
        }

        // Code path only reached here once all actions have finished (effect iterator reaches end)
        // GD.Print("All effects executed");
        EmitSignal(SignalName.ActionFinished);
    }

    private void OnTimerFinished()
    {
        _activeTimer.Timeout -= OnTimerFinished;
        _activeTimer = null;
        ExecuteNextEffect();
    }

    private void OnAnimationFinished()
    {
        _currentSprite.AnimationFinished -= OnAnimationFinished;
        actionAnimator.CleanupAnimation(_currentSprite);
        _currentSprite = null;
        ExecuteNextEffect();
    }

    private void OnDamagePopupFinished()
    {
        _currentPopup.FinishedDamagePopup -= OnDamagePopupFinished;
        ExecuteNextEffect();
    }
}
