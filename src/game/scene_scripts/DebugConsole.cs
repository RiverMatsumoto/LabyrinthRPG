using Godot;
using System;
using System.Collections.Generic;

#if DEBUG
public partial class DebugConsole : CanvasLayer
{
    [Export] public Control Root;
    [Export] public RichTextLabel Output;
    [Export] public LineEdit Input;

    private readonly Dictionary<string, Action> _commands = new();
    private BattleScene _battleScene;

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Always;

        Root.Visible = false;
        Input.TextSubmitted += OnSubmit;

        // find live BattleScene at runtime
        _battleScene = GetTree().GetFirstNodeInGroup("BattleScene") as BattleScene;

        RegisterCommands();
        Print("Debug console ready. Type /help");
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("DebugConsole"))
        {
            GD.Print("Showing Debug Console");
            ToggleConsoleVisibility();
        }
    }

    private void ToggleConsoleVisibility()
    {
        Root.Visible = !Root.Visible;
        if (Root.Visible)
        {
            Input.Clear();
            Input.GrabFocus();
            // Pause game while open
            GetTree().Paused = true;
        }
        else
        {
            GetTree().Paused = false;
        }
        GetViewport().SetInputAsHandled();
    }

    private void RegisterCommands()
    {
        _commands["/help"] = () =>
        {
            Print("Commands:");
            foreach (var k in _commands.Keys)
                Print($"  {k}");
        };
        _commands["/exit"] = () =>
        {
            _battleScene?.ClearAllPartyMembers();
            Print("Cleared party");
        };

        _commands["/AddDebugFrontRow"] = () =>
        {
            _battleScene?.AddPartyMemberFrontRow(new Battler());
            Print("Added party member to front row");
        };

        _commands["/AddDebugBackRow"] = () =>
        {
            _battleScene?.AddPartyMemberBackRow(new Battler());
            Print("Added party member to back row");
        };

        _commands["/ClearParty"] = () =>
        {
            _battleScene?.ClearAllPartyMembers();
            Print("Cleared party");
        };
    }

    private void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        Print($"> {text}");
        Input.Clear();

        if (_commands.TryGetValue(text.Trim(), out var cmd))
        {
            cmd.Invoke();
        }
        else
        {
            Print("Unknown command. Try /help");
        }
    }

    private void Print(string msg)
    {
        GD.Print("Test");
        Output.AppendText(msg + "\n");
        Output.ScrollToLine(Output.GetLineCount());
    }

    public void CloseDebugConsole()
    {
        ToggleConsoleVisibility();
    }
}
#endif
