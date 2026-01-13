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
    [Export] private DamagePopup _popup;
    private SceneTreeTimer _activeTimer;
    private BattleRunCtx _ctx;
    private ActionDef _currentAction = default!;
    private IEnumerator<IEffect> effectIterator;
    private PlaybackOptions playbackOptions;
    private Action _cancelWait;

    private bool _waitingAnim;
    private bool _waitingPopup;

    public override void _Ready()
    {
        playbackOptions = new PlaybackOptions();
    }

    public void Configure(BattleRunCtx ctx)
    {
        _ctx = ctx;
    }

    public void Execute(ActionDef action)
    {
        _currentAction = action;
        effectIterator = _currentAction.Effects.GetEnumerator();

        ExecuteNextEffect();
    }

    public void SkipNow()
    {
        if (_activeTimer != null)
        {
            _activeTimer.Timeout -= ExecuteNextEffect;
            _activeTimer = null;
        }

        // if (_waitingAnim)
        // {
        //     _anim.AnimationFinished -= OnAnimFinished;
        //     _waitingAnim = false;
        // }

        // if (_waitingPopup)
        // {
        //     _popup.Finished -= OnPopupFinished;
        //     _waitingPopup = false;
        // }

        ExecuteNextEffect();
    }

    private void ExecuteNextEffect()
    {
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
                    var timer = GetTree().CreateTimer(w.Seconds * playbackOptions.Speed);
                    timer.Timeout += ExecuteNextEffect;
                    return;

                case PlayAnim anim:
                    _waitingAnim = true;
                    Vector2 position = _ctx.TargetNodes[_ctx.Targets.First()]
                        .GetNode<TextureRect>("TextureRect")
                        .GetGlobalRect()
                        .GetCenter();
                    AnimatedSprite2D spriteNode = actionAnimator.PlayOnce(anim.AnimId, position, playbackOptions.Speed);
                    spriteNode.AnimationFinished += () =>
                    {
                        _cancelWait = null;
                        _waitingAnim = false;
                        actionAnimator.CleanupAnimation(spriteNode);
                        ExecuteNextEffect();
                    };

                    _cancelWait = () =>
                    {
                        actionAnimator.CleanupAnimation(spriteNode);
                        _cancelWait = null;
                    };
                    return;

                case PlayDamagePopup:
                    _waitingPopup = true;
                    void DamagePopupFinished()
                    {
                        _popup.Finished -= DamagePopupFinished;
                        _waitingPopup = false;
                        ExecuteNextEffect();
                    }
                    _popup.Finished += DamagePopupFinished; // implement this on DamagePopup

                    _cancelWait = () =>
                    {
                        _popup.Cancel();
                        DamagePopupFinished();
                        _cancelWait = null;
                    };
                    return;

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
}
