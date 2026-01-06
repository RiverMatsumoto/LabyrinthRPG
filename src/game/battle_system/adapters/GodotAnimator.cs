using System.Threading;
using System.Threading.Tasks;
using Godot;

public sealed class GodotAnimator(AnimationPlayer player) : IAnimator
{
    private readonly AnimationPlayer _player = player;

    public void Play(string anim) => _player.Play(anim);

    public async Task PlayAsync(string anim, CancellationToken ct)
    {
        _player.Play(anim);

        // Wait for the specific animation to finish
        while (true)
        {
            if (ct.IsCancellationRequested) break;
            var finished = await _player.ToSignal(_player, AnimationPlayer.SignalName.AnimationFinished);
            var name = (StringName)finished[0];
            if (name == anim) break;
        }
    }
}
