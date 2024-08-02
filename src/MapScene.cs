using Godot;

public partial class MapScene : Node3D
{
    [Signal] public delegate void MoveEndedEventHandler();
}
