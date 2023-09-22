using Godot;

public partial class TestSettings : Node
{
	public override void _Ready()
	{
		Run();
	}

	public void Run()
	{
		Settings.LoadSettings();
		GD.Print(Settings.data.Fps);
		Settings.SaveSettings();
	}
}
