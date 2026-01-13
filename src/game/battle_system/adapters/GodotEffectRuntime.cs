// using Godot;

// public sealed class GodotEffectRuntime(
//     ActionAnimator anim,
//     DamagePopup popup,
//     PlaybackOptions playback)
//     : IEffectRuntime
// {
//     private readonly ActionAnimator _anim = anim;
//     private readonly DamagePopup _popup = popup;
//     private readonly PlaybackOptions _playback = playback;

//     public PlaybackOptions Playback => _playback;

//     public void Log(string msg) => GD.Print(msg);

//     IEffectWait IEffectRuntime.WaitSeconds(float seconds)
//     {
//         return new WaitTimer(seconds);
//     }

//     IEffectWait IEffectRuntime.PlayAnim(string id, bool wait)
//     {
//         _anim.PlayOnce(id, id, Vector2.Zero, 1.0f);
//         AnimatedSprite2D sprite = _anim.GetNode<AnimatedSprite2D>("Sprite");
//         return wait ? new WaitAnimFinished(sprite) : new NoWait();
//     }

//     IEffectWait IEffectRuntime.ShowDamage(int amount)
//     {
//         _popup.ShowDamage(amount);
//         return new WaitDamagePopup();
//     }
// }
