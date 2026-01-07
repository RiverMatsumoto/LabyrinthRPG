using Godot;
using System.Threading;
using System.Threading.Tasks;

public partial class DamagePopup : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public Task ShowAsync(int amount, CancellationToken ct)
    {
        GD.Print($"DAMAGE POP UP {amount}");
        return Task.CompletedTask;
    }
}
