using System.Collections;
using System.Collections.Generic;
using Godot;

public interface IActionExecutor
{
    void Execute(ActionDef action, BattleRunCtx ctx);
    void SkipNow();
}

public sealed partial class GodotActionExecutor : Node, IActionExecutor
{
    [Signal] public delegate void ActionFinishedEventHandler();
    private BattleRunCtx _ctx = default!;
    private ActionDef _currentAction = default!;
    private IEnumerator<IEffect> effectIterator;

    [Export] private AnimationPlayer _anim = default!;
    [Export] private DamagePopup _popup = default!;
    private GodotEffectRuntime _runtime = default!;
    private SceneTreeTimer _activeTimer;
    private bool _waitingAnim;
    private bool _waitingPopup;

    public void Configure(PlaybackOptions playback)
    {
        _runtime = new GodotEffectRuntime(this, _anim, _popup, playback);
    }

    public void Execute(ActionDef action, BattleRunCtx ctx)
    {
        _currentAction = action;
        _ctx = new BattleRunCtx(ctx.Model, ctx.Source, ctx.Targets, ctx.DamageRegistry, _runtime);
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

        if (_waitingAnim)
        {
            _anim.AnimationFinished -= OnAnimFinished;
            _waitingAnim = false;
        }

        if (_waitingPopup)
        {
            _popup.Finished -= OnPopupFinished;
            _waitingPopup = false;
        }

        ExecuteNextEffect();
    }

    private void ExecuteNextEffect()
    {
        while (effectIterator.MoveNext())
        {
            var eff = effectIterator.Current; // easiest is: make ActionDef.Effects a List<IEffect>
            var wait = eff.Execute(_ctx);
            // _ctx.Runtime.Log($"Executing Effect: {eff}");

            if (wait == null)
                continue;

            switch (wait)
            {
                case NoWait:
                    continue;

                case WaitSeconds w:
                    _activeTimer = GetTree().CreateTimer(w.Seconds * _ctx.Runtime.Playback.Speed);
                    _activeTimer.Timeout += ExecuteNextEffect;
                    return;

                case WaitAnimFinished:
                    _waitingAnim = true;
                    _anim.AnimationFinished += OnAnimFinished;
                    return;

                case WaitDamagePopup:
                    _waitingPopup = true;
                    _popup.Finished += OnPopupFinished; // implement this on DamagePopup
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

    private void OnAnimFinished(StringName _)
    {
        _anim.AnimationFinished -= OnAnimFinished;
        _waitingAnim = false;
        ExecuteNextEffect();
    }

    private void OnPopupFinished()
    {
        _popup.Finished -= OnPopupFinished;
        _waitingPopup = false;
        ExecuteNextEffect();
    }
}
