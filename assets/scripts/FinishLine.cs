using Godot;
using System;

public partial class FinishLine : Node3D
{
	[Export] private Control _ui;
	[Export] private GameMode _gameMode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ui.Hide();
		_gameMode = _gameMode == null ? GetNode<GameMode>("game_mode") : _gameMode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEnter(Node3D body) {
		if (body is PhysicsCar car) {
			GD.Print("Crossed the finish line!");
			_ui.Show();
			_gameMode.EndGame();
		}
	}
}
