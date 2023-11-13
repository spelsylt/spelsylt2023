using Godot;
using System;

public partial class ScaleMap : Node3D
{
	private PhysicsCar _car;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Global.Instance.Car != null) {
			_car = Global.Instance.Car;
			GetTree().Root.AddChild(_car);
			_car.GlobalPosition = Vector3.Zero;
			_car.Disable();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
