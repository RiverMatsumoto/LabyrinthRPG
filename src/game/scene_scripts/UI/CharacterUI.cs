using System;
using Godot;

public partial class CharacterUI : BattlerUI
{
    [Export] RichTextLabel NameText;
    [Export] RichTextLabel HpValueText;
    [Export] RichTextLabel TpValueText;
    [Export] ProgressBar HpProgressBar;
    [Export] ProgressBar TpProgressBar;
    [Export] TextureRect StatusEffectTexture;

    public override void _Ready()
    {
        // Enable mouse input for click detection
        MouseFilter = MouseFilterEnum.Stop;
        GuiInput += OnGuiInput;
    }

    public override void PopulateData(Battler battler)
    {
        Battler = battler;
        Battler.Stats.Hp.OnChanged += (new_hp) => UpdateHealth();
        UpdateUI();
    }

    public override void UpdateUI()
    {
        NameText.Text = Battler.Stats.Name;
        UpdateTp();
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        HpValueText.Text = Battler.Stats.Hp.Value.ToString();
        HpProgressBar.Value = (float)Battler.Stats.Hp.Value / Battler.Stats.MaxHp.Value;
    }

    public void UpdateTp()
    {
        TpValueText.Text = Battler.Stats.Tp.ToString();
        TpProgressBar.Value = (float)Battler.Stats.Tp / Battler.Stats.MaxTp;
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

    public override void _ExitTree()
    {
        GuiInput -= OnGuiInput;
        if (Battler.Stats.Hp != null)
        {
            Battler.Stats.Hp.OnChanged -= (new_hp) => UpdateHealth();
        }
    }

    public override Vector2 GetTextureCenter() => GetGlobalRect().GetCenter();
}
