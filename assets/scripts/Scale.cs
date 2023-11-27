using Godot;
using System;

public partial class Scale : Node3D
{
	[Export] Node3D _needle;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetWeight(float weight) {
		_needle.Rotate(_needle.Transform.Basis.X, -0.01f);
	}
}
