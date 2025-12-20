using Godot;

[Tool]
public partial class BattleScene : Control
{
    public BattleModel Model { get; set; }

    [Export] HBoxContainer PlayerPartyFrontRowContainer;
    [Export] HBoxContainer PlayerPartyBackRowContainer;
    [Export] PackedScene characterUIPackedScene;

    public override void _Ready()
    {
        InitializeBattle();

        Model.playerPartyFrontRow.Add(new Battler());
        Model.playerPartyFrontRow.Add(new Battler());
        Model.playerPartyBackRow.Add(new Battler());
        Model.playerPartyBackRow.Add(new Battler());
        Model.enemyParty.Add(new Battler());
        Model.enemyParty.Add(new Battler());
        Model.enemyParty.Add(new Battler());

        InitializeUI();
    }

    public void InitializeBattle()
    {
        Model = new BattleModel();
    }

    public void InitializeUI()
    {
        foreach (var member in Model.playerPartyFrontRow)
            AddPartyMemberFrontRow(member);
        foreach (var member in Model.playerPartyBackRow)
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
}
