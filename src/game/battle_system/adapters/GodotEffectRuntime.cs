using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

// public static class GodotSignals
// {
//     public static async Task AwaitSignal(GodotObject emitter, StringName signal, CancellationToken ct)
//     {
//         if (!ct.CanBeCanceled)
//         {
//             await emitter.ToSignal(emitter, signal);
//             return;
//         }

//         var cancelTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
//         using var reg = ct.Register(static s => ((TaskCompletionSource)s!).TrySetResult(), cancelTcs);

//         var signalTask = emitter.ToSignal(emitter, signal).AsTask(); // converts SignalAwaiter -> Task
//         await Task.WhenAny(signalTask, cancelTcs.Task);

//         ct.ThrowIfCancellationRequested();
//         await signalTask; // propagate exceptions if any
//     }

//     public static async Task AsTask(this SignalAwaiter a) => await a;
// }

// public static class GodotWait
// {
//     public static Task NextFrame(Node node, CancellationToken ct)
//         => GodotSignals.AwaitSignal(node.GetTree(), SceneTree.SignalName.ProcessFrame, ct);

//     public static async Task DelaySeconds(Node node, double seconds, CancellationToken ct)
//     {
//         var timer = node.GetTree().CreateTimer(seconds);
//         await GodotSignals.AwaitSignal(timer, SceneTreeTimer.SignalName.Timeout, ct);
//     }

//     public static Task AnimFinished(AnimationPlayer ap, CancellationToken ct)
//         => GodotSignals.AwaitSignal(ap, AnimationPlayer.SignalName.AnimationFinished, ct);
// }

public sealed class GodotEffectRuntime(
    Node host,
    AnimationPlayer anim,
    DamagePopup popup,
    PlaybackOptions playback)
    : IEffectRuntime
{
    private readonly Node _host = host;
    private readonly AnimationPlayer _anim = anim;
    private readonly DamagePopup _popup = popup;
    private readonly PlaybackOptions _playback = playback;

    public PlaybackOptions Playback => _playback;

    public void Log(string msg) => GD.Print(msg);

    IEffectWait IEffectRuntime.WaitSeconds(float seconds)
    {
        return new WaitSeconds(seconds);
    }

    IEffectWait IEffectRuntime.PlayAnim(string id, bool wait)
    {
        anim.Play(id, customBlend: -1, customSpeed: playback.Speed);
        return wait ? new WaitAnimFinished() : new NoWait();
    }

    IEffectWait IEffectRuntime.ShowDamage(int amount)
    {
        return new WaitDamagePopup();
    }
}
