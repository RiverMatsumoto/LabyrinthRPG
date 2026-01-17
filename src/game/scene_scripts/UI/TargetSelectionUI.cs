
using System.Collections.Generic;
using Godot;

public partial class TargetSelectionUI : Control
{
    [Signal] public delegate void TargetSelectedEventHandler(Battler target);
    [Signal] public delegate void TargetSelectionCancelledEventHandler();

    public void ShowTargetSelection(Targeting rule, Battler source, BattleModel model)
    {

    }
    public void HighlightValidTargets(List<Battler> validTargets)
    {

    }
    public void HandleTargetClick(Battler target)
    {

    }
}
