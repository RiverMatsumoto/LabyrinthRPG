using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed partial class ActionSelectionMenu : Control
{
    [Signal] public delegate void SelectedActionEventHandler(string actionId);
    private List<Button> actionButtons = new();

    public override void _Ready()
    {
        foreach (Button button in GetChildren())
        {
            button.ButtonUp += () => SelectAction(button.Name.ToString());
            actionButtons.Add(button);
        }
        // BindNeighbors();
    }

    private void BindNeighbors()
    {
        foreach (var (button, index) in actionButtons.Select((button, index) => (button, index)))
        {
            // first item case
            if (index == 0)
            {
                button.FocusNeighborBottom = actionButtons[index + 1].GetPath();
            }
            // last item case
            else if (index == actionButtons.Count - 1)
            {
                button.FocusNeighborBottom = actionButtons[index - 1].GetPath();
            }
            else
            {
                button.FocusNeighborBottom = actionButtons[index + 1].GetPath();
                button.FocusNeighborBottom = actionButtons[index - 1].GetPath();
            }
        }
    }


    public void SelectAction(string actionId)
    {
        EmitSignal(SignalName.SelectedAction, actionId);
    }
}
