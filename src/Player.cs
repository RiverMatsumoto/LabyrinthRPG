using Godot;
using System;

public partial class Player : Node3D
{
	private float timeToMove = 0.2f;
	private bool isMoving = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isMoving || Game.State != GameState.Labyrinth) return;
		
		Vector2 direction = new Vector2();
		if (Input.IsActionPressed("MoveForward"))
			direction = new Vector2(1f, 0f);
		else if (Input.IsActionPressed("MoveBackward"))
			direction = new Vector2(-1f, 0f);
		else if (Input.IsActionPressed("MoveRight"))
			direction = new Vector2(0f, 1f);
		else if (Input.IsActionPressed("MoveLeft"))
			direction = new Vector2(0f, -1f);
		else return;
		isMoving = true;
		Vector3 finalPos = ToLocal(Position + new Vector3(direction.X, 0, direction.Y));
		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", finalPos, timeToMove)
			.AsRelative().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out).Finished += () => isMoving = false;
		tween.Play();
	}
}
