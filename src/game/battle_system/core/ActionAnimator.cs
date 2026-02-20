
using System.Collections.Generic;
using Godot;

public partial class ActionAnimator : Node
{
    [Export] private AnimationRegistry Registry;
    [Export] private Node UiRoot; // CanvasLayer or Control to attach sprites to
    [Export] private PackedScene animationScene;
    [Export] private PackedScene damagePopupScene;
    [Export] private float playerDeathFadeSeconds = 0.15f;
    [Export] private float enemyDeathFadeSeconds = 0.5f;
    public HashSet<AnimatedSprite2D> animationsRunning;

    public override void _Ready()
    {
        animationsRunning = new();
    }

    public AnimatedSprite2D PlayOnce(string anim, Vector2 screenPos, float speed)
    {
        if (Registry == null)
            throw new System.Exception("ActionAnimator: Registry not set.");

        SpriteFrames frames = Registry.GetFrames(anim);
        // speed
        GD.Print($"Playing animation: {anim} with speed {speed}");
        frames.SetAnimationSpeed("default", fps: 30);

        // force one-shot
        frames.SetAnimationLoop("default", false);

        AnimatedSprite2D sprite = animationScene.Instantiate<AnimatedSprite2D>();
        sprite.SpriteFrames = frames;
        sprite.GlobalPosition = screenPos;
        sprite.Frame = 0;

        UiRoot.AddChild(sprite);

        animationsRunning.Add(sprite);
        sprite.Play("default");
        return sprite;
    }

    public DamagePopup PlayDamagePopup(int amount, Vector2 screenPos, float speed)
    {
        DamagePopup popup = damagePopupScene.Instantiate<DamagePopup>();
        popup.GlobalPosition = screenPos;
        UiRoot.AddChild(popup);
        popup.ShowDamage(amount, speed);
        GD.Print($"Damage popup created at {screenPos}");
        return popup;
    }

    public void CleanupAnimation(AnimatedSprite2D anim)
    {
        animationsRunning.Remove(anim);
        anim.QueueFree();
    }

    public Tween PlayDeathAnimPlayer(BattlerUI target, float speed)
    {
        if (target == null)
            throw new System.Exception("ActionAnimator: target not set.");

        var tween = target.CreateTween();
        var toColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        tween.TweenProperty(
            target,
            "modulate",
            toColor,
            ScaleSeconds(playerDeathFadeSeconds, speed)
        ).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        return tween;
    }

    public Tween PlayDeathAnimEnemy(BattlerUI target, float speed)
    {
        if (target == null)
            throw new System.Exception("ActionAnimator: target not set.");

        var tween = target.CreateTween();
        var current = target.Modulate;
        var toColor = new Color(current.R, current.G, current.B, 0.0f);
        tween.TweenProperty(
            target,
            "modulate",
            toColor,
            ScaleSeconds(enemyDeathFadeSeconds, speed)
        ).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        return tween;
    }

    private static float ScaleSeconds(float seconds, float speed)
    {
        if (speed <= 0.0001f) return seconds;
        return seconds / speed;
    }
}
