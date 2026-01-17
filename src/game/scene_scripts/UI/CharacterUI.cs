using System;
using Godot;

public partial class CharacterUI : Control
{
    [Export] RichTextLabel NameText;
    [Export] RichTextLabel HpValueText;
    [Export] RichTextLabel TpValueText;
    [Export] ProgressBar HpProgressBar;
    [Export] ProgressBar TpProgressBar;
    [Export] TextureRect StatusEffectTexture;

    private Battler _battler;

    public void PopulateData(Battler battler)
    {
        _battler = battler;
        _battler.Stats.Hp.OnChanged += (new_hp) => UpdateHealth();
        UpdateUI();
    }

    public void UpdateUI()
    {
        NameText.Text = _battler.Stats.Name;
        UpdateTp();
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        HpValueText.Text = _battler.Stats.Hp.Value.ToString();
        HpProgressBar.Value = _battler.Stats.Hp.Value / _battler.Stats.MaxHp.Value;
    }

    public void UpdateTp()
    {
        TpValueText.Text = _battler.Stats.Tp.ToString();
        TpProgressBar.Value = _battler.Stats.Tp / _battler.Stats.MaxTp;
    }
}
