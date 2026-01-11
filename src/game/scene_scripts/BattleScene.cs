using System.Collections.Generic;
using Godot;

public partial class BattleScene : Control
{
    [Signal] public delegate void BattleFinishedEventHandler();
    [Export] private GameData gameData;
    [Export] private GodotActionExecutor actionExecutor;
    [Export] private EnemyRegistry enemyRegistry;

    // UI START
    [Export] private HBoxContainer PlayerPartyFrontRowContainer;
    [Export] private HBoxContainer PlayerPartyBackRowContainer;
    [Export] private PackedScene characterUIPackedScene;
    [Export] private PackedScene enemyUIPackedScene;
    [Export] private Control battleMenu;
    [Export] private HBoxContainer enemyContainer;

    [Export] private AnimationPlayer animationPlayer;
    [Export] private DamagePopup damagePopup;
    [Export] private TextureRect backgroundTexture;
    // UI END

    private GameState gameState;
    private Queue<ActionDef> _actionQueue;
    private IActionLibrary _actionLibrary;

    // Context for the current battle.
    public BattleRunCtx ctx;

    public override void _Ready()
    {
        // gameState =
        _actionQueue = new();
        backgroundTexture.Visible = false;
        // InitializeBattle(new EncounterData());
        // InitializeUI();
    }

    public void InitializeBattle(EncounterData encounterData)
    {
        GD.Print("=== Battle System Test Start ===");
        gameData.State = GameState.Battle;
        var battleModel = new BattleModel();
        battleModel.playerParty.AddToFrontRow(new Battler());
        battleModel.playerParty.AddToFrontRow(new Battler());
        battleModel.playerParty.AddToBackRow(new Battler());
        var enemy1 = new Battler(enemyRegistry.GetEnemy("squid_wizard"));
        var enemy2 = new Battler(enemyRegistry.GetEnemy("squid_wizard"));
        battleModel.enemyParty.AddToFrontRow(enemy1);
        battleModel.enemyParty.AddToFrontRow(enemy2);

        // Fake battle context
        var source = new Battler();
        var target = new Battler();

        var playback = new PlaybackOptions();

        ctx = new BattleRunCtx(
            model: battleModel,
            source: source,
            targets: new[] { target },
            damageRegistry: new TestDamageRegistry(),
            runtime: new GodotEffectRuntime(
                anim: animationPlayer,
                popup: damagePopup,
                playback: playback
            )
        );
        actionExecutor.Configure(ctx, animationPlayer, damagePopup);
        actionExecutor.ActionFinished += OnActionFinished;

        InitializeUI();

    }

    public void InitializeUI()
    {
        backgroundTexture.Visible = true;
        battleMenu.Visible = true;
        foreach (var member in ctx.Model.playerParty.GetFrontRowMembers())
            AddPartyMemberFrontRow(member);
        foreach (var member in ctx.Model.playerParty.GetBackRowMembers())
            AddPartyMemberBackRow(member);
        foreach (var enemy in ctx.Model.enemyParty)
            AddEnemyToBattle(enemy);

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

    public void AddEnemyToBattle(Battler enemy)
    {
        EnemyUI enemyUI = enemyUIPackedScene.Instantiate<EnemyUI>();
        enemyUI.PopulateData(enemy);
        enemyContainer.AddChild(enemyUI);
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

    public void ExecuteActions()
    {
        GD.Print("=== Running Battle Actions ===");
        // Load actions
        _actionLibrary = new ActionLibrary("data/actions.yaml");

        // enqueue test actions
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
        actionExecutor.Execute(action);
    }

    private void OnActionFinished()
    {
        StartNextAction();
    }

    public void CleanupBattle()
    {
        // Hide players
        // Hide BattleUI
        // Hide background
        // emit battle finished

    }

    public void GoToMap()
    {
    }
}
