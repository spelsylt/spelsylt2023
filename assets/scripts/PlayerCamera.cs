using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	public Node3D target;

	[Export] private float _lerpSpeed = 10.0f;
	[Export] private float _distanceToTarget = 5.0f;
	[Export] private float _heightToTarget = 5.0f;
	[Export] private float _angleOffset = 10.0f;
	[Export] private float _RotationSpeed = 0.005f;
	[Export] private Node3D _outerGimbal;
	[Export] private Node3D _innerGimbal;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (target == null) 
		{
			return;
		}
		
		GlobalTransform = GlobalTransform.InterpolateWith(target.GlobalTransform, _lerpSpeed * (float) delta);
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion) {
			var mouseEvent = (InputEventMouseMotion) @event;
			_outerGimbal.RotateObjectLocal(Vector3.Up, -mouseEvent.Relative.X * _RotationSpeed);
			_innerGimbal.RotateObjectLocal(Vector3.Right, -mouseEvent.Relative.Y * _RotationSpeed);
		}
    }
}
