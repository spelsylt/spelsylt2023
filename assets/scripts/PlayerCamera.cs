using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	[Export] public VehicleBody3D _car;

    [Export] private float _mouseCaptureDelay = 1.0f;
    [Export] private float _cameraStiffness = 1.0f;
	[Export] private float _lerpSpeed = 10.0f;
	[Export] private float _distanceToTarget = 5.0f;
	[Export] private float _heightToTarget = 5.0f;
	[Export] private float _angleOffset = 10.0f;
	[Export] private float _RotationSpeed = 0.005f;
	[Export] private Node3D _outerGimbal;
	[Export] private Node3D _innerGimbal;

    private double _mouseControleCountdown = 0.0f;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_car == null) 
		{
			return;
		}
		
		GlobalTransform = GlobalTransform.InterpolateWith(_car.GlobalTransform, _lerpSpeed * (float) delta);
        if (_mouseControleCountdown <= 0.0f) {
            var newYaw = Mathf.LerpAngle(_outerGimbal.Rotation.Y, MathF.PI, (float)delta / _cameraStiffness);
            var newPitch = Mathf.LerpAngle(_innerGimbal.Rotation.X, -MathF.PI / 8, (float)delta / _cameraStiffness);

            _outerGimbal.Rotation = new Vector3(0.0f, newYaw, 0.0f);
            _innerGimbal.Rotation = new Vector3(newPitch, 0.0f, 0.0f);
        } else {
            _mouseControleCountdown -= delta;
        }
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseEvent) {
			_outerGimbal.RotateObjectLocal(Vector3.Up, -mouseEvent.Relative.X * _RotationSpeed);
			_innerGimbal.RotateObjectLocal(Vector3.Right, -mouseEvent.Relative.Y * _RotationSpeed);
            _mouseControleCountdown = _mouseCaptureDelay;
        }
    }
}
