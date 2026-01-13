
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AnimationRegistry : Resource
{
    // Godot can serialize Resources in Dictionaries when typed like this.
    [Export]
    public Godot.Collections.Dictionary<string, SpriteFrames> Animations { get; set; } = new();

    public bool Has(string key) => Animations.ContainsKey(key);

    public SpriteFrames GetFrames(string key)
    {
        if (!Animations.TryGetValue(key, out var frames) || frames == null)
            throw new KeyNotFoundException($"AnimationRegistry missing key '{key}'.");
        return frames;
    }

    public AnimatedSprite2D ApplyTo(AnimatedSprite2D sprite, string key, string defaultAnim = null)
    {
        var frames = GetFrames(key);
        sprite.SpriteFrames = frames;

        // If caller didn't provide a specific anim name, pick something sensible.
        var animName = defaultAnim;
        if (string.IsNullOrEmpty(animName))
        {
            var names = frames.GetAnimationNames();
            animName = names.Length > 0 ? names[0] : "";
        }

        if (!string.IsNullOrEmpty(animName) && frames.HasAnimation(animName))
            sprite.Animation = animName;

        return sprite;
    }

    public void Play(AnimatedSprite2D sprite, string key, string animName, bool oneShot = false, bool restart = true)
    {
        var frames = GetFrames(key);
        sprite.SpriteFrames = frames;

        if (!frames.HasAnimation(animName))
            throw new KeyNotFoundException($"SpriteFrames '{key}' has no animation '{animName}'.");

        // sprite.OneShot = oneShot;
        // if (restart) sprite.Frame = 0;
        sprite.Play(animName);
        sprite.AnimationFinished += () =>
        {
            sprite.Stop();
            sprite.AnimationFinished -= null;
        };
    }

    public Array<string> Keys()
    {
        var outKeys = new Array<string>();
        foreach (var k in Animations.Keys) outKeys.Add(k);
        return outKeys;
    }
}
