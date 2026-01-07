using Godot;
using System;

public partial class EnemyUI : Control
{
    [Export] public Sprite2D sprite;
    public Battler battler;

    public override void _Ready()
    {
    }
}
