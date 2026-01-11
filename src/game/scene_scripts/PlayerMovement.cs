using System;
using Godot;

public partial class PlayerMovement : Node3D
{
    [Export] private GridMap gridMap;
    [Export] private float moveTime = 0.10f;
    [Export] private Timer cooldownTimerNode;
    [Export] private Tween.TransitionType easeType;
    [Export] private GameData gameData;
    [Signal] public delegate void OnMoveCompleteEventHandler();

    private bool isMoving;
    private TurnData td;

    private Vector3I cell; // authoritative grid position
    private readonly Vector3 cellCenterOffset = new Vector3(0, 0.5f, 0);

    public override void _Ready()
    {
        td = new TurnData();

        cooldownTimerNode = GetNode<Timer>("Timer");
        cooldownTimerNode.OneShot = true;
        cooldownTimerNode.Timeout += OnCooldownFinished;

        cell = new Vector3I(0, 0, 0);
        SnapToCell(cell);
    }

    public override void _Process(double delta)
    {
        ProcessTurn(delta);
        if (isMoving || gameData.State != GameState.Labyrinth) return;

        Vector3I step = Vector3I.Zero;

        if (Input.IsActionPressed("MoveForward")) step = ForwardStep();
        else if (Input.IsActionPressed("MoveBackward")) step = -ForwardStep();
        else if (Input.IsActionPressed("MoveRight")) step = RightStep();
        else if (Input.IsActionPressed("MoveLeft")) step = -RightStep();
        else if (Input.IsActionPressed("TurnLeft")) { Turn(-1); return; }
        else if (Input.IsActionPressed("TurnRight")) { Turn(1); return; }
        else return;

        var targetCell = cell + step;

        if (!IsWalkableFloor(targetCell))
            return;

        MoveToCell(targetCell);
    }

    private void MoveToCell(Vector3I targetCell)
    {
        isMoving = true;

        Vector3 targetWorld = CellToWorld(targetCell);

        Tween tween = CreateTween();
        tween.TweenProperty(this, "global_position", targetWorld, moveTime)
             .SetTrans(easeType)
             .SetEase(Tween.EaseType.Out);

        tween.Finished += () =>
        {
            cell = targetCell;       // commit discrete position
            SnapToCell(cell);        // kill float drift
            StartCooldown();
            EmitSignal(SignalName.OnMoveComplete);
        };
    }

    private bool IsWalkableFloor(Vector3I c)
    {
        int item = gridMap.GetCellItem(c);
        if (item == GridMap.InvalidCellItem) return false;

        string name = gridMap.MeshLibrary.GetItemName(item);
        return name == "Floor";
    }

    private Vector3 CellToWorld(Vector3I c)
    {
        Vector3 local = gridMap.MapToLocal(c) + cellCenterOffset;
        return gridMap.ToGlobal(local);
    }

    private void SnapToCell(Vector3I c) => GlobalPosition = CellToWorld(c);

    // Facing as a 0..3 index (0 = +Z or -Z depending on your choice)
    // This version assumes "forward" is -Z in world when Rotation.Y == 0.
    private int FacingIndex()
    {
        // Convert radians to quarter turns, rounding to nearest.
        int q = Mathf.RoundToInt((float)(Rotation.Y / (Mathf.Pi / 2)));
        q = ((q % 4) + 4) % 4;
        return q;
    }

    private Vector3I ForwardStep()
    {
        return FacingIndex() switch
        {
            0 => new Vector3I(1, 0, 0), // ForwardX
            1 => new Vector3I(0, 0, -1), // RightZ
            2 => new Vector3I(-1, 0, 0),  // BackwardX
            _ => new Vector3I(0, 0, 1),  // Left)
        };
    }

    private Vector3I RightStep()
    {
        return FacingIndex() switch
        {
            0 => new Vector3I(0, 0, 1),
            1 => new Vector3I(1, 0, 0),
            2 => new Vector3I(0, 0, -1),
            _ => new Vector3I(-1, 0, 0),
        };
    }

    private Vector3I StepFromFacing(bool forward)
    {
        int f = FacingIndex();

        // forward vectors for indices 0,1,2,3
        Vector3I[] forwardSteps =
        {
            new Vector3I(1, 0, 0), // Forward
            new Vector3I(0, 0, -1), // Right
            new Vector3I(-1, 0, 0),  // Backward
            new Vector3I(0, 0, 1),  // Left
        };

        var s = forwardSteps[f];
        return forward ? s : -s;
    }

    private Vector3I StepStrafe(bool right)
    {
        // Right is forward rotated +90 degrees
        int f = FacingIndex();
        int r = (f + 3) % 4;
        Vector3I[] rightSteps =
        {
            new Vector3I(0, 0, 1),
            new Vector3I(1, 0, 0),
            new Vector3I(0, 0, -1),
            new Vector3I(-1, 0, 0),
        };

        var s = rightSteps[f];
        return right ? s : -s;
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
        float t = (float)(td.TimeTurning / moveTime);
        float y = (float)Mathf.LerpAngle((float)td.StartRot, (float)td.EndRot, (float)EaseOutSine(t));

        Rotation = new Vector3(Rotation.X, y, Rotation.Z);

        if (td.TimeTurning > moveTime)
        {
            td.Turning = false;
            Rotation = new Vector3(Rotation.X, (float)td.EndRot, Rotation.Z);
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        isMoving = true;
        cooldownTimerNode.Start();
    }

    private void OnCooldownFinished()
    {
        isMoving = false;
    }

    private struct TurnData
    {
        public bool Turning;
        public double StartRot;
        public double EndRot;
        public double TimeTurning;
    }

    private double EaseOutSine(double x) => Math.Sin(Math.Clamp(x, 0.0, 1.0) * (Math.PI / 2));
}
