using Godot;
using System;

public partial class MainMenuUI : BoxContainer
{
	[Export] private Control _settingsUI;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnClickStart() {
		GetTree().ChangeSceneToFile("res://scenes/map1.tscn");
	}

	public void OnClickSettings() {
		_settingsUI.Show();
		Hide();
	}

	public void OnClickQuit() {
		GetTree().Quit();
	}
}
