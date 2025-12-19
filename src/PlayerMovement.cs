using System;
using Godot;

public partial class PlayerMovement : Node3D
{
    private GridMap gridMap;
    private float moveTime = 0.15f;
    private bool isMoving = false;
    [Export] private double moveCooldown = 0.01;
    private double cooldownTimer = 0;
    private Timer cooldownTimerNode;
    private void StartTurn() => td.Turning = true;
    private TurnData td;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        td = new TurnData();
        gridMap = GetNode<GridMap>("../GridMap");
        cooldownTimerNode = GetNode<Timer>("Timer");
        cooldownTimerNode.OneShot = true;
        cooldownTimerNode.Timeout += () => isMoving = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        ProcessTurn(delta);

        if (isMoving || Game.State != GameState.Labyrinth) return;

        Vector3 dir;
        if (Input.IsActionPressed("MoveForward"))
            dir = GlobalTransform.Basis.X;
        else if (Input.IsActionPressed("MoveBackward"))
            dir = -GlobalTransform.Basis.X;
        else if (Input.IsActionPressed("MoveRight"))
            dir = GlobalTransform.Basis.Z;
        else if (Input.IsActionPressed("MoveLeft"))
            dir = -GlobalTransform.Basis.Z;
        else if (Input.IsActionPressed("TurnLeft"))
        {
            Turn(-1);
            return;
        }
        else if (Input.IsActionPressed("TurnRight"))
        {
            Turn(1);
            return;
        }
        else return;

        // Vector3 localDirection = ToLocal(Position + finalPos);
        var finalPos = Position + dir;
        Vector3I globalFinalPos = new Vector3I(
            Mathf.RoundToInt(finalPos.X),
            Mathf.RoundToInt(finalPos.Y),
            Mathf.RoundToInt(finalPos.Z));
        GD.Print(Position);
        GD.Print(dir);
        GD.Print(globalFinalPos);
        // GD.Print(localDirection);

        int cellItem = gridMap.GetCellItem(globalFinalPos);
        if (cellItem == GridMap.InvalidCellItem) return;
        string cellName = gridMap.MeshLibrary.GetItemName(cellItem);
        if (cellName != "Floor") return;

        isMoving = true;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", dir, moveTime)
            .AsRelative().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out)
            .Finished += () => StartCooldown();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("DEBUG_3"))
        {
            GD.Print($"Right: {ToGlobal(GlobalTransform.Basis.X)}");
            GD.Print($"Up: {ToGlobal(GlobalTransform.Basis.Y)}");
            GD.Print($"Forward: {ToGlobal(GlobalTransform.Basis.Z)}");
        }
    }

    public void Turn(float direction)
    {
        isMoving = true;
        StartTurn(direction);
    }

    public void StartTurn(float direction)
    {
        isMoving = true;
        td.Turning = true;
        td.StartRot = Rotation.Y;
        td.EndRot = Mathf.Wrap(Rotation.Y - (Mathf.Pi / 2) * direction, -Mathf.Pi, Mathf.Pi);
        td.TimeTurning = 0;
    }

    public void ProcessTurn(double delta)
    {
        if (!td.Turning) return;

        td.TimeTurning += delta;
        float y = (float)Mathf.LerpAngle(td.StartRot, td.EndRot, EaseOutSine(td.TimeTurning / moveTime));
        Rotation = new Vector3(Rotation.X, y, Rotation.Z);
        if (td.TimeTurning > moveTime)
        {
            td.Turning = false;
            StartCooldown();
            Rotation = new Vector3(Rotation.X, (float)td.EndRot, Rotation.Z);
        }
    }

    private void StartCooldown()
    {
        isMoving = true;
        cooldownTimerNode.Start();
    }

    private struct TurnData
    {
        public bool Turning;
        public double StartRot;
        public double EndRot;
        public double TimeTurning;
    }

    private double EaseOutSine(double x) => Math.Sin(x * (Math.PI / 2));
}
