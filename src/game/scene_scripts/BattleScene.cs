using System;
using System.Collections.Generic;
using Godot;

public partial class BattleScene : Control
{
    [Signal] public delegate void BattleFinishedEventHandler();

    [Export] public GameData gameData;
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
    [Export] public TextureRect backgroundTexture;
    // UI END

    [Export] public DamageCalculatorRegistry damageCalculatorRegistry;
    [Export] public ActionRegistry _actionRegistry;
    private BattleStateMachine battleStateMachine;
    private Queue<ActionDef> actionQueue;
    private EncounterData encounterData;

    // Context for the current battle.
    public BattleRunCtx ctx;

    public override void _Ready()
    {
        LoadSaveData();
        // gameState =
        actionQueue = new();
        backgroundTexture.Hide();
        battleStateMachine = new BattleStateMachine(this);
    }

    public void LoadSaveData()
    {

    }


    public void StartBattle(EncounterData encounterData)
    {
        this.encounterData = encounterData;
        ChangeBattleState(typeof(InitializeBattlePhase));
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
        actionQueue.Enqueue(_actionRegistry.Get("FireAttack"));

        // start chain of actions until empty
        StartNextAction();
    }

    private void StartNextAction()
    {
        if (actionQueue.Count <= 0)
        {
            GD.Print("All actions complete");
            return;
        }

        var action = actionQueue.Dequeue();
        // GD.Print($"Executing action: {action.Id}");
        actionExecutor.Execute(action);
    }

    public void OnActionFinished()
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

    public void ChangeBattleState(Type state) => battleStateMachine.ChangeState(state);

    public void GoToMap()
    {
        // deal with exiting battle cleanly
        CleanupBattle();
        // hide battle scene and go back to the map
    }
}
