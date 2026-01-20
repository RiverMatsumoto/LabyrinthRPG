using Godot;

public sealed partial class ActionSelectionMenu : Control
{
    [Signal] public delegate void SelectedActionEventHandler(string actionId);

    public override void _Ready()
    {
        foreach (Button button in GetChildren())
        {
            button.ButtonUp += () => SelectAction(button.Name.ToString());
        }
    }

    public void SelectAction(string actionId)
    {
        EmitSignal(nameof(SelectedAction), actionId);
    }
}
