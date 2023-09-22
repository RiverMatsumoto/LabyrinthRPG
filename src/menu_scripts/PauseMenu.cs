using Godot;

public partial class PauseMenu : Control
{
	private bool subMenuOpened = false;
	private Node openSubMenu = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Menu"))
			if (Game.State == GameState.Labyrinth || Game.State == GameState.Town)
				OpenMenu();
			else if (Game.State == GameState.PauseMenu)
				CloseMenu();
	}

	public void OpenMenu()
	{
		 Game.State = GameState.PauseMenu;
		 Visible = true;
	}
	
	public void CloseMenu()
	{
		 if (subMenuOpened)
		 { 
			   openSubMenu.Disconnect("tree_exited", 
					new Callable(this, "_SubMenuClosed"));
			   subMenuOpened = false;
			   openSubMenu.QueueFree();
		 }
		 Visible = false;
		 Game.State = GameState.Labyrinth;
	}

	public void OpenSubMenu(string subMenuPath)
	{
		PackedScene subMenuNode = ResourceLoader.Load<PackedScene>(subMenuPath);
		Node subMenu = subMenuNode.Instantiate();
		Callable callback = new Callable(this, "_SubMenuClosed");
		subMenu.Connect("tree_exited", callback);
		GetTree().Root.AddChild(subMenu);
		
		openSubMenu = subMenu;
		Visible = false;
		subMenuOpened = true;
	}
	
	public void _SubMenuClosed()
	{
		Visible = true;
		subMenuOpened = false;
	}

	public void _on_item_pressed() => OpenSubMenu("res://src/scenes/ItemMenu.tscn");

	public void _on_skill_pressed() => OpenSubMenu("res://src/scenes/SkillMenu.tscn");

	public void _on_status_pressed() => OpenSubMenu("res://src/scenes/StatusMenu.tscn");

	public void _on_visibility_changed()
	{
		if (Visible == false) return;

		// scale tween
		// Position = new Vector2(-100, 0);
		// Tween tween = CreateTween();
		// tween.TweenProperty(this, "position", Vector2.Zero, 0.01f)
		// 	.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
	}

	public void _on_back_button_pressed()
	{
		CloseMenu();
	}
}
