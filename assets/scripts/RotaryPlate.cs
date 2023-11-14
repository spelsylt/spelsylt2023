using Godot;
using System;

public partial class RotaryPlate : Node3D
{
	[Export] float _speed = 10.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotateY(_speed * (float) delta);
	}
}
