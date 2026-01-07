using Godot;

public partial class StatusMenu : Control
{
    public override void _Ready()
    {
        // Game game = GetNode<Game>("/root/Game");
        // CharacterBase c = game.CurrentParty.Characters[0];
        // // TODO load characters
        // Label name = GetNode<Label>("Stats/Name");
        // Label hp = GetNode<Label>("Stats/HP");
        // // name.Text = c.Name;
        // hp.Text = c.Hp.ToString();
    }

    public void _on_back_button_pressed()
    {
        QueueFree();
    }

}
