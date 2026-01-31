using Godot;

public partial class EnemyUI : BattlerUI
{
    [Export] private VBoxContainer vBoxContainer;
    [Export] private TextureProgressBar healthBar;
    [Export] private TextureRect sprite;

    public override void _Ready()
    {
        // Enable mouse input for click detection
        MouseFilter = MouseFilterEnum.Stop;
        GuiInput += OnGuiInput;
    }

    public override void PopulateData(Battler enemy)
    {
        Battler = enemy;
        sprite.Texture = enemy.Sprite;
        Battler.Stats.Hp.OnChanged += UpdateHealth;
        UpdateUI();
    }

    public override void UpdateUI()
    {
        UpdateHealth();
    }

    public void UpdateHealth(int hp) => UpdateHealth();

    public void UpdateHealth()
    {
        // float division not ints
        GD.Print($"Updating health bar: {(double)Battler.Stats.Hp.Value / Battler.Stats.MaxHp.Value}");
        healthBar.Value = (double)Battler.Stats.Hp.Value / Battler.Stats.MaxHp.Value;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.BattlerClicked, Battler);
            }
        }
    }

    public Battler GetBattler() => Battler;

    // destructor
    public override void _ExitTree()
    {
        GuiInput -= OnGuiInput;
        if (Battler?.Stats?.Hp != null)
        {
            Battler.Stats.Hp.OnChanged -= UpdateHealth;
        }
    }

    public override Vector2 GetTextureCenter() => sprite.GetGlobalRect().GetCenter();
}
