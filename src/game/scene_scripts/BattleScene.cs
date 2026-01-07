using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class BattleScene : Control
{

    [Export] HBoxContainer PlayerPartyFrontRowContainer;
    [Export] HBoxContainer PlayerPartyBackRowContainer;
    [Export] PackedScene characterUIPackedScene;
    [Export] DamagePopup damagePopup;
    public BattleRunCtx ctx;

    public override void _Ready()
    {
        InitializeBattle();

        ctx.Model.playerParty.AddToFrontRow(new Battler());
        ctx.Model.playerParty.AddToFrontRow(new Battler());
        ctx.Model.playerParty.AddToBackRow(new Battler());
        ctx.Model.enemyParty.AddToFrontRow(new Battler());
        ctx.Model.enemyParty.AddToFrontRow(new Battler());

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
                anim: GetNode<AnimationPlayer>("AnimationPlayer"),
                popup: GetNode<DamagePopup>("DamagePopup"),
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

    public async Task RunActionsAsync()
    {
        GD.Print("=== Running Battle Actions ===");
        // Load actions
        var library = new ActionLibrary();

        // Execute
        var actions = new List<ActionDef>();
        actions.Add(library.Get("BasicAttack"));
        var actionCt = new CancellationTokenSource();
        foreach (var a in actions)
        {
            // debug print
            GD.Print($"[Debug] basic attack effects: {a.Effects.ToString()}");
            await ActionExecutor.ExecuteAsync(a, ctx, actionCt.Token);
        }

        GD.Print("=== Battle System Test End ===");
    }
}
