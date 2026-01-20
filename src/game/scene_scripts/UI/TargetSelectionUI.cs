using System.Collections.Generic;
using Godot;

public partial class TargetSelectionUI : Node
{
    [Signal] public delegate void TargetSelectedEventHandler(Battler target);
    [Signal] public delegate void TargetSelectionCancelledEventHandler();

    [Export] public AnimatedSprite2D targetCaret;


    public override void _Ready()
    {
        targetCaret.Hide();
    }

    public void ShowTargetSelection(Targeting rule, Battler source, BattleModel model)
    {
        targetCaret.Show();
        var validTargets = TargetSelector.GetValidTargets(rule, source, model);
    }

    public void HighlightValidTargets(List<Battler> validTargets)
    {

    }

    public void HandleTargetClick(Battler target)
    {

    }
}
