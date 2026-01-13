using Godot;

public partial class CharacterUI : Control
{
    [Export] RichTextLabel NameText;
    [Export] RichTextLabel HpValueText;
    [Export] RichTextLabel TpValueText;
    [Export] TextureRect StatusEffectTexture;

    private Battler _battler;

    public void PopulateData(Battler battler)
    {
        _battler = battler;
        UpdateUI();
    }

    public void UpdateUI()
    {
        NameText.Text = _battler.Stats.Name;
        HpValueText.Text = _battler.Stats.Hp.ToString();
        TpValueText.Text = _battler.Stats.Tp.ToString();
    }
}
