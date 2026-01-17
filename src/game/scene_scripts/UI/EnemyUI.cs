using Godot;

public partial class EnemyUI : VBoxContainer
{
    [Export] private TextureProgressBar healthBar;
    [Export] private TextureRect sprite;
    public Battler Battler;

    public void PopulateData(Battler enemy)
    {
        Battler = enemy;
        sprite.Texture = enemy.Sprite;
        Battler.Stats.Hp.OnChanged += UpdateHealth;
        UpdateUI();
    }

    public void UpdateUI()
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


    // destructor
    public override void _ExitTree()
    {
        Battler.Stats.Hp.OnChanged -= UpdateHealth;
    }

}
