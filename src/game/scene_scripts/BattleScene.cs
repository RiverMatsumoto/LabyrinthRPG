using System;
using System.Collections.Generic;
using Godot;

public partial class BattleScene : Control
{
    [Signal] public delegate void BattleFinishedEventHandler();

    [Export] public GodotActionExecutor actionExecutor;
    [Export] public EnemyRegistry enemyRegistry;

    // UI START
    [Export] public CanvasLayer PlayerPartyUI;
    [Export] public CanvasLayer EnemyPartyUI;
    [Export] public CanvasLayer BattleMenuUI;
    [Export] public CanvasLayer EffectsUI;
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
    [Export] public ActionRegistry actionRegistry;
    [Export] public BattlerBaseRegistry battlerBaseRegistry;
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
        GD.Print("=== Battle System Start ===");
        // unhide battle ui canvas layers on for battle scene, gets
        ShowAllUI();

        var (playerParty, enemyParty) = SetupParties(encounterData);
        var battlerUINodes = SetupBattleUI(playerParty, enemyParty);

        InitializeBattleSystem(playerParty, enemyParty, battlerUINodes);
    }

    private (Party playerParty, Party enemyParty) SetupParties(EncounterData encounterData)
    {
        // TODO: Replace with actual character creator/loader later
        Party playerParty = battlerBaseRegistry.GetDebugParty();

        Party enemyParty = new Party(maxMembersRow: 5);
        for (int i = 0; i < encounterData.Enemies.Count; i++)
        {
            var enemy = new Battler(enemyRegistry.GetEnemy(encounterData.Enemies[i]));
            enemyParty.AddToFrontRow(enemy);
        }

        return (playerParty, enemyParty);
    }

    private Dictionary<Battler, BattlerUI> SetupBattleUI(Party playerParty, Party enemyParty)
    {
        Dictionary<Battler, BattlerUI> battlerUINodes = new();

        foreach (Battler member in playerParty.GetFrontRowMembers())
        {
            var memberNode = AddBattlerUI(member, characterUIPackedScene, PlayerPartyFrontRowContainer);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (Battler member in playerParty.GetBackRowMembers())
        {
            var memberNode = AddBattlerUI(member, characterUIPackedScene, PlayerPartyBackRowContainer);
            battlerUINodes.Add(member, memberNode);
        }
        foreach (Battler enemy in enemyParty)
        {
            var enemyNode = AddBattlerUI(enemy, enemyUIPackedScene, enemyContainer);
            battlerUINodes.Add(enemy, enemyNode);
        }

        return battlerUINodes;
    }

    private void InitializeBattleSystem(Party playerParty, Party enemyParty, Dictionary<Battler, BattlerUI> battlerUINodes)
    {
        var battleModel = new BattleModel
        {
            playerParty = playerParty,
            enemyParty = enemyParty
        };

        ctx = new BattleRunCtx(
            model: battleModel,
            turnPlan: new TurnPlan(),
            targetNodes: battlerUINodes,
            damageRegistry: damageCalculatorRegistry,
            rng: new RandomNumberGenerator()
        );

        actionExecutor.Configure(ctx);

        // Initialize battle state machine BEFORE it starts transitioning states
        battleStateMachine = new BattleStateMachine(this);
        battleStateMachine.Initialize();
    }

    public void InitializeUI()
    {
        backgroundTexture.Show();
        battleMenu.Show();
    }

    private BattlerUI AddBattlerUI(Battler battler, PackedScene scene, Control container)
    {
        BattlerUI ui = scene.Instantiate<BattlerUI>();
        ui.PopulateData(battler);
        ui.BattlerClicked += OnBattlerUIClicked;
        container.AddChild(ui);
        return ui;
    }

    // Kept for compatibility if other scripts call these specific methods,
    // but they now use the generic helper.
    public BattlerUI AddPartyMemberFrontRow(Battler member) =>
        AddBattlerUI(member, characterUIPackedScene, PlayerPartyFrontRowContainer);

    public BattlerUI AddPartyMemberBackRow(Battler member) =>
        AddBattlerUI(member, characterUIPackedScene, PlayerPartyBackRowContainer);

    public BattlerUI AddEnemyToBattle(Battler enemy) =>
        AddBattlerUI(enemy, enemyUIPackedScene, enemyContainer);

    private void OnBattlerUIClicked(BattlerUI battlerUI)
    {
        // Forward click to target selection UI if it's active
        if (targetSelectionUI != null && targetSelectionUI.IsActive())
        {
            targetSelectionUI.HandleTargetClick(battlerUI.Battler);
        }
    }

    public void HideAllUI()
    {
        EffectsUI.Hide();
        PlayerPartyUI.Hide();
        EnemyPartyUI.Hide();
        BattleMenuUI.Hide();
    }

    public void ShowAllUI()
    {
        EffectsUI.Show();
        PlayerPartyUI.Show();
        EnemyPartyUI.Show();
        BattleMenuUI.Show();
    }

    public void ClearAllPartyMembers()
    {
        ClearContainer(PlayerPartyFrontRowContainer);
        ClearContainer(PlayerPartyBackRowContainer);
        ClearContainer(enemyContainer);
    }

    private void ClearContainer(Control container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is BattlerUI battlerUI)
            {
                battlerUI.BattlerClicked -= OnBattlerUIClicked;
            }
            child.QueueFree();
        }
    }

    public void CleanupBattle()
    {
        // Hide players
        // Hide BattleUI
        // Hide background
        HideAllUI();
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
