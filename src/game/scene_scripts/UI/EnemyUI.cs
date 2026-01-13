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
    }
}
