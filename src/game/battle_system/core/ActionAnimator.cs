
using System.Collections.Generic;
using Godot;

public partial class ActionAnimator : Node
{
    [Export] private AnimationRegistry Registry;
    [Export] private Node UiRoot; // CanvasLayer or Control to attach sprites to
    [Export] private PackedScene animationScene;
    [Export] private PackedScene damagePopupScene;
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
}
