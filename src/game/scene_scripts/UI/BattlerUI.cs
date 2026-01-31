using Godot;

public abstract partial class BattlerUI : Control
{
    [Signal] public delegate void BattlerClickedEventHandler(BattlerUI battlerUI);
    public Battler Battler { get; protected set; }

    /// used to initialize the BattlerUI data and let the ui hook into certain data within the Battler
    public abstract void PopulateData(Battler battler);
    public abstract void UpdateUI();
    public abstract Vector2 GetTextureCenter();
}
