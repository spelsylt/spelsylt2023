using Godot;
using System;

public partial class UICar : Control
{
	[Export] Label _speedLabel;
	[Export] Label _rpmLabel;
	[Export] Label _GearLabel;

	[Export] PhysicsCar _car;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var speed = _car.LinearVelocity.Length() * 3.6f; // Multiply by 3.6 to convert m/s to Km/h
		_speedLabel.Text = (int) Mathf.Floor(speed) + " Km/h";
		_rpmLabel.Text = "RPM:" + Mathf.Floor(_car.RPM);
		_GearLabel.Text = "" + _car.Gear;
	}
}
