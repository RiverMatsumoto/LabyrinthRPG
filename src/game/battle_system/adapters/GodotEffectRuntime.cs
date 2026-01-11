using Godot;

public sealed class GodotEffectRuntime(
    AnimationPlayer anim,
    DamagePopup popup,
    PlaybackOptions playback)
    : IEffectRuntime
{
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
        _anim.Play(id, customBlend: -1, customSpeed: _playback.Speed);
        return wait ? new WaitAnimFinished() : new NoWait();
    }

    IEffectWait IEffectRuntime.ShowDamage(int amount)
    {
        _popup.ShowDamage(amount);
        return new WaitDamagePopup();
    }
}
