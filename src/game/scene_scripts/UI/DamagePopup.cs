using Godot;

public partial class DamagePopup : RichTextLabel
{
    [Signal]
    public delegate void FinishedDamagePopupEventHandler();

    public override void _Process(double delta)
    {
        // move up slowly
        Position += new Vector2(0, (float)(-delta * 100.0));
    }

    public void ShowDamage(int amount, float speed)
    {
        // set timer for 1 second, move upwards slowly until timeout
        SceneTreeTimer timer = GetTree().CreateTimer(0.5 / speed);
        timer.Timeout += EndDamagePopup;

        BbcodeEnabled = true;
        HorizontalAlignment = HorizontalAlignment.Center;
        Text = $"[b]{amount}[/b]";
        GD.Print($"DAMAGE POP UP {amount}");
    }

    public void EndDamagePopup()
    {
        GD.Print("End damage popup");
        EmitSignal(nameof(FinishedDamagePopup));
        QueueFree();
    }
}
