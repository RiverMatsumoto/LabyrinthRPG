using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Handles target selection UI for battle actions.
/// Uses BattleRunCtx as the single source of truth.
/// </summary>
public partial class TargetSelectionUI : Node
{
    [Export] public AnimatedSprite2D targetCaret;

    [Signal] public delegate void TargetSelectedEventHandler(Battler target);
    [Signal] public delegate void TargetSelectionCancelledEventHandler();

    private BattleRunCtx _ctx = default!;

    private List<Battler> _validTargets = new();
    private int _currentIndex = 0;
    private bool _isActive = false;
    private Targeting _currentRule;

    // Grid navigation structure: [row][column] = battler
    private List<List<Battler>> _targetGrid = new();

    public override void _Ready()
    {
        targetCaret.Hide();
        SetProcessInput(false);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isActive || _validTargets.Count == 0)
            return;

        if (@event.IsActionPressed("ui_cancel"))
        {
            CancelSelection();
            GetViewport().SetInputAsHandled();
            return;
        }

        if (@event.IsActionPressed("ui_accept"))
        {
            ConfirmSelection();
            GetViewport().SetInputAsHandled();
            return;
        }

        HandleGridNavigation(@event);
    }

    /// <summary>
    /// Shows the target selection UI and initializes valid targets from ctx.
    /// </summary>
    public void ShowTargetSelection(Targeting rule, Battler source, BattleRunCtx ctx)
    {
        _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        _currentRule = rule;

        // bool sourceIsEnemy = _ctx.Model.enemyParty.Contains(source);

        _validTargets = TargetSelector.GetValidTargets(
            rule,
            source,
            _ctx.Model,
            sourceIsEnemy: false
        );

        if (_validTargets.Count == 0)
        {
            GD.PrintErr("No valid targets found!");
            return;
        }

        _isActive = true;
        SetProcessInput(true);

        BuildTargetGrid();
        HighlightValidTargets(_validTargets);

        _currentIndex = 0;
        UpdateCaretPosition();
        targetCaret.Show();
        targetCaret.Play("default");

        GD.Print($"Target selection active. {_validTargets.Count} valid targets.");
    }

    public void HideTargetSelection()
    {
        _isActive = false;
        SetProcessInput(false);
        targetCaret.Hide();
        ClearHighlights();
        _validTargets.Clear();
        _targetGrid.Clear();
        _currentIndex = 0;
        _ctx = default!;
    }

    private void BuildTargetGrid()
    {
        _targetGrid.Clear();

        if (_currentRule == Targeting.SingleEnemy || _currentRule == Targeting.AllEnemies)
        {
            // Enemies in one row
            var row = _validTargets.ToList();
            if (row.Count > 0) _targetGrid.Add(row);
        }
        else if (_currentRule == Targeting.SingleAlly || _currentRule == Targeting.AllAllies)
        {
            // Allies: front then back
            var party = _ctx.Model.playerParty;

            var front = new List<Battler>();
            foreach (var m in party.GetFrontRowMembers())
                if (_validTargets.Contains(m)) front.Add(m);
            if (front.Count > 0) _targetGrid.Add(front);

            var back = new List<Battler>();
            foreach (var m in party.GetBackRowMembers())
                if (_validTargets.Contains(m)) back.Add(m);
            if (back.Count > 0) _targetGrid.Add(back);
        }
        else if (_currentRule == Targeting.Self)
        {
            _targetGrid.Add(_validTargets.ToList());
        }

        GD.Print($"Built target grid with {_targetGrid.Count} rows");
    }

    private void HandleGridNavigation(InputEvent @event)
    {
        if (_targetGrid.Count == 0)
            return;

        // Find current position in grid
        int currentRow = 0, currentCol = 0;
        bool found = false;

        for (int row = 0; row < _targetGrid.Count; row++)
        {
            for (int col = 0; col < _targetGrid[row].Count; col++)
            {
                if (_targetGrid[row][col] == _validTargets[_currentIndex])
                {
                    currentRow = row;
                    currentCol = col;
                    found = true;
                    break;
                }
            }
            if (found) break;
        }

        int newRow = currentRow;
        int newCol = currentCol;

        if (@event.IsActionPressed("ui_left"))
        {
            newCol--;
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_right"))
        {
            newCol++;
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_up"))
        {
            newRow--;
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_down"))
        {
            newRow++;
            GetViewport().SetInputAsHandled();
        }
        else
        {
            return;
        }

        if (_targetGrid.Count == 0 || _targetGrid[newRow].Count == 0)
            return;

        // Wrap rows
        if (newRow < 0) newRow = _targetGrid.Count - 1;
        else if (newRow >= _targetGrid.Count) newRow = 0;

        // Wrap cols within row
        if (newCol < 0) newCol = _targetGrid[newRow].Count - 1;
        else if (newCol >= _targetGrid[newRow].Count) newCol = 0;

        var newTarget = _targetGrid[newRow][newCol];
        _currentIndex = _validTargets.IndexOf(newTarget);
        UpdateCaretPosition();
    }

    private void UpdateCaretPosition()
    {
        if (_currentIndex < 0 || _currentIndex >= _validTargets.Count)
            return;

        var target = _validTargets[_currentIndex];

        if (_ctx?.TargetNodes == null || !_ctx.TargetNodes.TryGetValue(target, out var targetNode))
        {
            GD.PrintErr($"Target node not found for battler: {target.Stats?.Name ?? "Unknown"}");
            return;
        }

        Rect2 targetRect = targetNode.GetGlobalRect();
        Vector2 caretSize = targetCaret.SpriteFrames.GetFrameTexture("default", 0).GetSize();

        targetCaret.GlobalPosition = new Vector2(
            targetRect.Position.X + targetRect.Size.X / 2 - caretSize.X / 2,
            targetRect.Position.Y - 20
        );
    }

    public void HighlightValidTargets(List<Battler> validTargets)
    {
        foreach (var t in validTargets)
        {
            if (_ctx.TargetNodes.TryGetValue(t, out var node))
                node.Modulate = new Color(1.2f, 1.2f, 1.2f, 1.0f);
        }
    }

    private void ClearHighlights()
    {
        if (_ctx?.TargetNodes == null) return;
        foreach (var kv in _ctx.TargetNodes)
            kv.Value.Modulate = Colors.White;
    }

    private void ConfirmSelection()
    {
        if (_currentIndex < 0 || _currentIndex >= _validTargets.Count)
            return;

        var selected = _validTargets[_currentIndex];
        GD.Print($"Target confirmed: {selected.Stats?.Name ?? "Unknown"}");
        EmitSignal(SignalName.TargetSelected, selected);
    }

    private void CancelSelection()
    {
        GD.Print("Target selection cancelled");
        EmitSignal(SignalName.TargetSelectionCancelled);
    }

    public void HandleTargetClick(Battler target)
    {
        if (!_isActive || !_validTargets.Contains(target))
        {
            GD.Print("Click on invalid or inactive target ignored");
            return;
        }

        _currentIndex = _validTargets.IndexOf(target);
        UpdateCaretPosition();
        ConfirmSelection();
    }

    public Battler GetCurrentTarget()
        => (_currentIndex >= 0 && _currentIndex < _validTargets.Count) ? _validTargets[_currentIndex] : null;

    public bool IsActive() => _isActive;
}
