using System;
using System.Collections.Generic;
using Godot;

public partial class BattleScene : Control
{
    [Signal] public delegate void BattleFinishedEventHandler();

    [Export] public GodotActionExecutor actionExecutor;
    [Export] public EnemyRegistry enemyRegistry;

    // UI START
    [Export] public HBoxContainer PlayerPartyFrontRowContainer;
    [Export] public HBoxContainer PlayerPartyBackRowContainer;
    [Export] public PackedScene characterUIPackedScene;
    [Export] public PackedScene enemyUIPackedScene;
    [Export] public Control battleMenu;
    [Export] public HBoxContainer enemyContainer;
    [Export] public TargetSelectionUI targetSelectionUI;
    [Export] public ActionSelectionMenu actionSelectionMenu;
    [Export] public TextureRect backgroundTexture;
    // UI END

    [Export] public DamageCalculatorRegistry damageCalculatorRegistry;
    [Export] public ActionRegistry _actionRegistry;
    private BattleStateMachine battleStateMachine;
    private Queue<ActionDef> actionQueue;
    public EncounterData encounterData;

    // Context for the current battle.
    public BattleRunCtx ctx;

    public override void _Ready()
    {
        LoadSaveData();
        // gameState =
        actionQueue = new();
        backgroundTexture.Hide();
        actionSelectionMenu.Hide();
    }

    public void LoadSaveData()
    {

    }


    public void StartBattle(EncounterData encounterData)
    {
        this.encounterData = encounterData;

        GD.Print("=== Battle System Test Start ===");

        // havent made a character creator tool yet
        // this comes wayyy way later after I design characters and enemies
        // enemies and classes need to be created together
        var battleModel = new BattleModel();
        battleModel.playerParty.AddToFrontRow(new Battler());
        battleModel.playerParty.AddToFrontRow(new Battler());
        battleModel.playerParty.AddToBackRow(new Battler());

        // load enemies from the encounter data
        for (int i = 0; i < encounterData.Enemies.Count; i++)
        {
            var enemy = new Battler(enemyRegistry.GetEnemy(encounterData.Enemies[i]));
            battleModel.enemyParty.AddToFrontRow(enemy);
        }

        Dictionary<Battler, Control> battlerUINodes = new();
        foreach (Battler member in battleModel.playerParty.GetFrontRowMembers())
        {
            var memberNode = AddPartyMemberFrontRow(member);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (Battler member in battleModel.playerParty.GetBackRowMembers())
        {
            var memberNode = AddPartyMemberBackRow(member);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (Battler enemy in battleModel.enemyParty)
        {
            var enemyNode = AddEnemyToBattle(enemy);
            battlerUINodes.Add(enemy, enemyNode);
        }

        var rng = new RandomNumberGenerator();

        ctx = new BattleRunCtx(
            model: battleModel,
            turnPlan: new TurnPlan(),
            targetNodes: battlerUINodes,
            damageRegistry: damageCalculatorRegistry,
            rng: rng
        );

        actionExecutor.Configure(ctx);

        // Initialize battle state machine BEFORE it starts transitioning states
        // This prevents null reference when InitializeBattlePhase calls ChangeBattleState
        battleStateMachine = new BattleStateMachine(this);
        battleStateMachine.Initialize();
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
        charUI.BattlerClicked += OnBattlerUIClicked;
        PlayerPartyFrontRowContainer.AddChild(charUI);
        return charUI;
    }

    public Control AddPartyMemberBackRow(Battler member)
    {
        CharacterUI charUI = characterUIPackedScene.Instantiate<CharacterUI>();
        charUI.PopulateData(member);
        charUI.BattlerClicked += OnBattlerUIClicked;
        PlayerPartyBackRowContainer.AddChild(charUI);
        return charUI;
    }

    public Control AddEnemyToBattle(Battler enemy)
    {
        EnemyUI enemyUI = enemyUIPackedScene.Instantiate<EnemyUI>();
        enemyUI.PopulateData(enemy);
        enemyUI.BattlerClicked += OnBattlerUIClicked;
        enemyContainer.AddChild(enemyUI);
        return enemyUI;
    }

    private void OnBattlerUIClicked(Battler battler)
    {
        // Forward click to target selection UI if it's active
        if (targetSelectionUI != null && targetSelectionUI.IsActive())
        {
            targetSelectionUI.HandleTargetClick(battler);
        }
    }

    public void ClearAllPartyMembers()
    {
        foreach (var member in PlayerPartyFrontRowContainer.GetChildren())
        {
            if (member is CharacterUI charUI)
            {
                charUI.BattlerClicked -= OnBattlerUIClicked;
            }
            member.QueueFree();
        }
        foreach (var member in PlayerPartyBackRowContainer.GetChildren())
        {
            if (member is CharacterUI charUI)
            {
                charUI.BattlerClicked -= OnBattlerUIClicked;
            }
            member.QueueFree();
        }
        foreach (var member in enemyContainer.GetChildren())
        {
            if (member is EnemyUI enemyUI)
            {
                enemyUI.BattlerClicked -= OnBattlerUIClicked;
            }
            member.QueueFree();
        }
    }

    public void CleanupBattle()
    {
        // Hide players
        // Hide BattleUI
        // Hide background
        // emit battle finished

    }

    public void ChangeBattleState(Type state) => battleStateMachine.ChangeState(state);

    public void GoToMap()
    {
        // deal with exiting battle cleanly
        CleanupBattle();
        // hide battle scene and go back to the map
    }
}
