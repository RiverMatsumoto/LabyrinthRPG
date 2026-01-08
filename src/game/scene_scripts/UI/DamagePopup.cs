using Godot;

public partial class DamagePopup : Control
{
    [Signal]
    public delegate void FinishedEventHandler();


    public void ShowDamage(int amount)
    {
        GD.Print($"DAMAGE POP UP {amount}");
    }
}
