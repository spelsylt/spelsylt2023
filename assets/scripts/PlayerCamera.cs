using Godot;
using System;

public partial class PlayerCamera : Camera3D
{
	public Node3D target;
	public Node3D position;
	[Export] private float _lerpSpeed = 10.0f;
	[Export] private float _distanceToTarget = 5.0f;
	[Export] private float _heightToTarget = 5.0f;
	[Export] private float _angleOffset = 10.0f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (target == null || position == null) 
		{
			return;
		}
		GlobalTransform = GlobalTransform.InterpolateWith(position.GlobalTransform, _lerpSpeed * (float) delta);
		LookAt(target.Position);
	}
}
