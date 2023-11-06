using Godot;
using System;

public partial class GameMode : Node
{
	[Export] private PhysicsCar _car;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_car.Freeze = true;
		// TODO count down 3 2 1 start then unfreeze that car and start a timer.
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
