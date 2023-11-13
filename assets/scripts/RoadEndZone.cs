using Godot;
using System;

public partial class RoadEndZone : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnEnter(Node3D body) {
		if (body is AICar car) {
			// TODO move AICar away from the map
			car.Disable();
		}
	}
}
