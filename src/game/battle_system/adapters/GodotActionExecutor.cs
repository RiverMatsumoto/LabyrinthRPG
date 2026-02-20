using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface IActionExecutor
{
    bool IsRunning { get; }
    void Configure(BattleRunCtx ctx);

    // Execute exactly one planned action (one actor + targets + ActionDef)
    void ExecutePlanned(BattleAction planned);

    void SkipNow();
}

public sealed partial class GodotActionExecutor : Node, IActionExecutor
{
    [Signal] public delegate void ActionFinishedEventHandler();

    [Export] private ActionAnimator actionAnimator = default!;
    [Export] private string playerDeathAnimId = "death_player";
    [Export] private string enemyDeathAnimId = "death_enemy";

    public bool IsRunning => _isRunning;

    private BattleRunCtx _ctx = default!;
    private bool _isRunning;

    private BattleAction _planned = default!;
    private ActionDef _currentAction = default!;
    private IEnumerator<IEffect> _effectIterator;

    private DamagePopup _currentPopup;
    private SceneTreeTimer _activeTimer;
    private AnimatedSprite2D _currentSprite;
    private Tween _currentTween;

    private enum ExecutionState { Idle, WaitingTimer, WaitingAnim, WaitingPopup, WaitingDeathAnim, WaitingDeathTween }
    private ExecutionState _state = ExecutionState.Idle;

    private readonly HashSet<Battler> _handledDeaths = new();
    private readonly Queue<Battler> _pendingDeaths = new();

    public void Configure(BattleRunCtx ctx) => _ctx = ctx;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
            SkipNow();
    }

    public void ExecutePlanned(BattleAction planned)
    {
        if (_ctx == null)
            throw new InvalidOperationException("GodotActionExecutor: Configure(ctx) must be called before ExecutePlanned().");

        if (_isRunning)
            throw new InvalidOperationException("GodotActionExecutor: already running an action.");

        _isRunning = true;

        _planned = planned;
        _ctx.Source = planned.Source;
        _ctx.Targets = planned.Targets;

        _currentAction = planned.ActionDef;
        _effectIterator = _currentAction.Effects.GetEnumerator();

        ExecuteNextEffect();
    }

    public void SkipNow()
    {
        switch (_state)
        {
            case ExecutionState.WaitingTimer:
                if (_activeTimer != null) _activeTimer.Timeout -= OnTimerFinished;
                _activeTimer = null;
                break;

            case ExecutionState.WaitingAnim:
                if (_currentSprite != null) _currentSprite.AnimationFinished -= OnAnimationFinished;
                if (_currentSprite != null) actionAnimator.CleanupAnimation(_currentSprite);
                _currentSprite = null;
                break;

            case ExecutionState.WaitingPopup:
                if (_currentPopup != null)
                {
                    _currentPopup.FinishedDamagePopup -= OnDamagePopupFinished;
                    _currentPopup.EndDamagePopup();
                }
                _currentPopup = null;
                break;
            case ExecutionState.WaitingDeathAnim:
                if (_currentSprite != null) _currentSprite.AnimationFinished -= OnDeathAnimationFinished;
                if (_currentSprite != null) actionAnimator.CleanupAnimation(_currentSprite);
                _currentSprite = null;
                break;
            case ExecutionState.WaitingDeathTween:
                if (_currentTween != null) _currentTween.Finished -= OnDeathTweenFinished;
                _currentTween?.Kill();
                _currentTween = null;
                break;
        }

        _state = ExecutionState.Idle;
        if (_state == ExecutionState.WaitingDeathAnim)
            ExecuteNextDeathAnim();
        else
            ExecuteNextEffect();
    }

    private void ExecuteNextEffect()
    {
        _state = ExecutionState.Idle;

        if (_effectIterator == null)
        {
            FinishAction();
            return;
        }

        while (_effectIterator.MoveNext())
        {
            var eff = _effectIterator.Current;
            var wait = eff.Execute(_ctx);
            GD.Print($"Executing Effect: {eff}");

            var playback = Util.GetGameGlobals(this).GameSettings.PlaybackOptions;

            switch (wait)
            {
                case NoWait:
                    continue;

                case WaitSeconds w:
                    _state = ExecutionState.WaitingTimer;
                    _activeTimer = GetTree().CreateTimer(ScaleSeconds(w.Seconds, playback.Speed));
                    _activeTimer.Timeout += OnTimerFinished;
                    return;

                case PlayAnim anim:
                    _state = ExecutionState.WaitingAnim;

                    var target = _ctx.Targets.First();
                    var pos = _ctx.TargetNodes[target].GetTextureCenter();
                    _currentSprite = actionAnimator.PlayOnce(anim.AnimId, pos, playback.Speed);
                    _currentSprite.AnimationFinished += OnAnimationFinished;
                    return;

                case PlayDamagePopup popup:
                    var t = _ctx.Targets.First();
                    var dmgPos = _ctx.TargetNodes[t].GetTextureCenter();

                    _currentPopup = actionAnimator.PlayDamagePopup(popup.Amount, dmgPos, playback.Speed);

                    if (popup.Wait)
                    {
                        _state = ExecutionState.WaitingPopup;
                        _currentPopup.FinishedDamagePopup += OnDamagePopupFinished;
                        return;
                    }

                    // no-wait popup: keep going
                    continue;

                default:
                    GD.PushError($"Unknown wait type: {wait.GetType().Name}");
                    continue;
            }
        }

        BeginDeathSequenceOrFinish();
    }

    private void BeginDeathSequenceOrFinish()
    {
        _state = ExecutionState.Idle;
        _pendingDeaths.Clear();

        foreach (var battler in _ctx.Model.playerParty.Concat(_ctx.Model.enemyParty))
        {
            if (!battler.IsAlive && !_handledDeaths.Contains(battler))
                _pendingDeaths.Enqueue(battler);
        }

        if (_pendingDeaths.Count == 0)
        {
            FinishAction();
            return;
        }

        ExecuteNextDeathAnim();
    }

    private void ExecuteNextDeathAnim()
    {
        if (_pendingDeaths.Count == 0)
        {
            FinishAction();
            return;
        }

        var battler = _pendingDeaths.Dequeue();
        _handledDeaths.Add(battler);

        var node = _ctx.TargetNodes[battler];

        bool isEnemy = _ctx.Model.enemyParty.Contains(battler);
        var speed = Util.GetGameGlobals(this).GameSettings.PlaybackOptions.Speed;

        if (isEnemy)
        {
            _state = ExecutionState.WaitingDeathTween;
            _currentTween = actionAnimator.PlayDeathAnimEnemy(node, speed);
            _currentTween.Finished += OnDeathTweenFinished;
            return;
        }

        _state = ExecutionState.WaitingDeathTween;
        _currentTween = actionAnimator.PlayDeathAnimPlayer(node, speed);
        _currentTween.Finished += OnDeathTweenFinished;
    }

    private void FinishAction()
    {
        _state = ExecutionState.Idle;
        _isRunning = false;

        _effectIterator?.Dispose();
        _effectIterator = null;

        EmitSignal(SignalName.ActionFinished);
    }

    private static double ScaleSeconds(double seconds, float speed)
    {
        // If speed > 1 means “faster”, divide time.
        // If your semantics are opposite, flip this back.
        if (speed <= 0.0001f) return seconds;
        return seconds / speed;
    }

    private void OnTimerFinished()
    {
        if (_activeTimer != null) _activeTimer.Timeout -= OnTimerFinished;
        _activeTimer = null;
        ExecuteNextEffect();
    }

    private void OnAnimationFinished()
    {
        if (_currentSprite != null) _currentSprite.AnimationFinished -= OnAnimationFinished;
        if (_currentSprite != null) actionAnimator.CleanupAnimation(_currentSprite);
        _currentSprite = null;
        ExecuteNextEffect();
    }

    private void OnDamagePopupFinished()
    {
        if (_currentPopup != null) _currentPopup.FinishedDamagePopup -= OnDamagePopupFinished;
        _currentPopup = null;
        ExecuteNextEffect();
    }

    private void OnDeathAnimationFinished()
    {
        if (_currentSprite != null) _currentSprite.AnimationFinished -= OnDeathAnimationFinished;
        if (_currentSprite != null) actionAnimator.CleanupAnimation(_currentSprite);
        _currentSprite = null;
        ExecuteNextDeathAnim();
    }

    private void OnDeathTweenFinished()
    {
        if (_currentTween != null) _currentTween.Finished -= OnDeathTweenFinished;
        _currentTween = null;
        ExecuteNextDeathAnim();
    }
}
