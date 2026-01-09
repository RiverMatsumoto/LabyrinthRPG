using Godot;
using System;

public partial class SystemsMediator : Node
{
    [Export] private MapScene mapScene;
    [Export] private BattleScene battleScene;
    [Export] private TextboxManager textboxManager;
    [Export] private PauseMenu pauseMenu;
    [Export] private DebugConsole debugConsole;

    public override void _Ready()
    {
        //
        // Map -> Battle
        mapScene.encounterSystemScene.EncounterTriggered += data =>
        {
            battleScene.InitializeBattle(data);
        };

        // Battle -> Map (example)
        // battleScene.BattleEnded += result =>
        // {
        //     mapScene.OnBattleEnded(result);
        // };

        // // Battle -> Textbox (example)
        // battleScene.RequestDialogue += dialogue =>
        // {
        //     textboxManager.Open(dialogue);
        // };

    }
    public override void _EnterTree()
    {
        GameServices.Build();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
