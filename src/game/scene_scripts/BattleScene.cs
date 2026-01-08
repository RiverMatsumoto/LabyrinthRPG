using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class BattleScene : Control
{

    [Export] HBoxContainer PlayerPartyFrontRowContainer;
    [Export] HBoxContainer PlayerPartyBackRowContainer;
    [Export] PackedScene characterUIPackedScene;
    [Export] AnimationPlayer animationPlayer;
    [Export] DamagePopup damagePopup;
    [Export] GodotActionExecutor actionExecutor;

    private Queue<ActionDef> _actionQueue;
    private IActionLibrary _actionLibrary;

    // Context for the current battle.
    public BattleRunCtx ctx;

    public override void _Ready()
    {
        _actionQueue = new();
        InitializeBattle();

        ctx.Model.playerParty.AddToFrontRow(new Battler());
        ctx.Model.playerParty.AddToFrontRow(new Battler());
        ctx.Model.playerParty.AddToBackRow(new Battler());
        ctx.Model.enemyParty.AddToFrontRow(new Battler());
        ctx.Model.enemyParty.AddToFrontRow(new Battler());

        actionExecutor.Configure(ctx.Runtime.Playback);
        actionExecutor.ActionFinished += OnActionFinished;

        InitializeUI();
    }

    public void InitializeBattle()
    {
        GD.Print("=== Battle System Test Start ===");
        // Fake battle context
        var source = new Battler();
        var target = new Battler();

        var playback = new PlaybackOptions();

        ctx = new BattleRunCtx(
            model: new BattleModel(),
            source: source,
            targets: new[] { target },
            damageRegistry: new TestDamageRegistry(),
            runtime: new GodotEffectRuntime(
                host: this,
                anim: animationPlayer,
                popup: damagePopup,
                playback: playback
            )
        );
    }

    public void InitializeUI()
    {
        foreach (var member in ctx.Model.playerParty)
            AddPartyMemberFrontRow(member);
        foreach (var member in ctx.Model.playerParty)
            AddPartyMemberBackRow(member);
    }

    public void AddPartyMemberFrontRow(Battler member)
    {
        CharacterUI charUI = characterUIPackedScene.Instantiate<CharacterUI>();
        charUI.PopulateData(member);
        PlayerPartyFrontRowContainer.AddChild(charUI);
    }
    public void AddPartyMemberBackRow(Battler member)
    {
        CharacterUI charUI = characterUIPackedScene.Instantiate<CharacterUI>();
        charUI.PopulateData(member);
        PlayerPartyBackRowContainer.AddChild(charUI);
    }

    public void ClearAllPartyMembers()
    {
        foreach (var member in PlayerPartyFrontRowContainer.GetChildren())
            member.QueueFree();
        foreach (var member in PlayerPartyBackRowContainer.GetChildren())
            member.QueueFree();
    }

    public override void _Process(double delta)
    {
    }

    private void LoadBattleState()
    {

    }

    public void ExecuteActions()
    {
        GD.Print("=== Running Battle Actions ===");
        // Load actions
        _actionLibrary = new ActionLibrary();

        // Execute
        _actionQueue.Enqueue(_actionLibrary.Get("BasicAttack"));
        _actionQueue.Enqueue(_actionLibrary.Get("FireAttack"));

        // start chain of actions until empty
        StartNextAction();
    }

    private void StartNextAction()
    {
        if (_actionQueue.Count <= 0)
        {
            GD.Print("All actions complete");
            return;
        }

        var action = _actionQueue.Dequeue();
        // GD.Print($"Executing action: {action.Id}");
        actionExecutor.Execute(action, ctx);
    }

    private void OnActionFinished()
    {
        StartNextAction();
    }
}
