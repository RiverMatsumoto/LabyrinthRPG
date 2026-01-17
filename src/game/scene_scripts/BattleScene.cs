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

    [Export] private TextureRect backgroundTexture;
    // UI END

    private Queue<ActionDef> _actionQueue;
    private IActionLibrary _actionLibrary;
    private IDamageCalculatorRegistry damageCalculatorRegistry;

    // Context for the current battle.
    public BattleRunCtx ctx;

    public override void _Ready()
    {
        // gameState =
        _actionQueue = new();
        backgroundTexture.Hide();
        _actionLibrary = new ActionLibrary("data/actions.yaml");
        damageCalculatorRegistry = new DamageCalculatorRegistry();
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
        Dictionary<Battler, Control> battlerUINodes = new();
        foreach (var member in battleModel.playerParty.GetFrontRowMembers())
        {
            var memberNode = AddPartyMemberFrontRow(member);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (var member in battleModel.playerParty.GetBackRowMembers())
        {
            var memberNode = AddPartyMemberBackRow(member);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (var enemy in battleModel.enemyParty)
        {
            var enemyNode = AddEnemyToBattle(enemy);
            battlerUINodes.Add(enemy, enemyNode);
        }

        // Fake battle context
        var source = battleModel.playerParty.GetFrontRowMember(0);
        var target = enemy1;

        var playback = new PlaybackOptions();

        ctx = new BattleRunCtx(
            model: battleModel,
            source: source,
            targets: new List<Battler> { target },
            targetNodes: battlerUINodes,
            damageRegistry: damageCalculatorRegistry,
            rng: new RandomNumberGenerator()
        );
        actionExecutor.Configure(ctx);
        actionExecutor.ActionFinished += OnActionFinished;

        InitializeUI();
    }

    public void InitializeUI()
    {
        backgroundTexture.Show();
        battleMenu.Show();

    }

    public Control AddPartyMemberFrontRow(Battler member)
    {
        CharacterUI charUI = characterUIPackedScene.Instantiate<CharacterUI>();
        charUI.PopulateData(member);
        PlayerPartyFrontRowContainer.AddChild(charUI);
        return charUI;
    }

    public Control AddPartyMemberBackRow(Battler member)
    {
        CharacterUI charUI = characterUIPackedScene.Instantiate<CharacterUI>();
        charUI.PopulateData(member);
        PlayerPartyBackRowContainer.AddChild(charUI);
        return charUI;
    }

    public Control AddEnemyToBattle(Battler enemy)
    {
        EnemyUI enemyUI = enemyUIPackedScene.Instantiate<EnemyUI>();
        enemyUI.PopulateData(enemy);
        enemyContainer.AddChild(enemyUI);
        return enemyUI;
    }

    public void ClearAllPartyMembers()
    {
        foreach (var member in PlayerPartyFrontRowContainer.GetChildren())
            member.QueueFree();
        foreach (var member in PlayerPartyBackRowContainer.GetChildren())
            member.QueueFree();
    }

    public void ExecuteActions()
    {
        GD.Print("=== Running Battle Actions ===");
        // Load actions

        // enqueue test actions
        // _actionQueue.Enqueue(_actionLibrary.Get("BasicAttack"));
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
        actionExecutor.ActionFinished -= OnActionFinished;
    }

    public void ChangeState(BattleState state)
    {

    }

    public void GoToMap()
    {
    }
}
