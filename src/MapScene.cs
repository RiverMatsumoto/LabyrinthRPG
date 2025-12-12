using Godot;

public partial class MapScene : Node3D
{
    [Signal] public delegate void MoveEndedEventHandler();

    [Export] GridMap gridMap;
    
    public override void _Ready()
    {
        Vector3I startCell = new Vector3I(0, 0, 0);
        int cell = gridMap.GetCellItem(startCell);
        string name = gridMap.MeshLibrary.GetItemName(cell);
        GD.Print(name);
    }
}
