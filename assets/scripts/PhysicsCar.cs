using Godot;
using System;
using System.Collections.Generic;

public partial class PhysicsCar : VehicleBody3D
{
	[Export] private float _acceleration = 10f;
	[Export] private float _maxRPM = 500;
	[Export] private float _maxTorque = 50;
	[Export] private float _steeringSpeed = 5.0f;
	[Export] private Node3D _cameraPosition;
	[Export] private PlayerCamera _playerCamera;

	private List<VehicleWheel3D> _drivingWheels;

    public override void _Ready()
    {
        base._Ready();
		_playerCamera.target = this;
		_playerCamera.position = _cameraPosition;
		_drivingWheels = GetDrivingWheels();
    }

    public override void _PhysicsProcess(double delta)
    {
        Steering = Mathf.Lerp(Steering, Input.GetAxis("steer_right", "steer_left") * 0.4f, _steeringSpeed * (float)delta);
		var acceleration = Input.GetAxis("brake_reverse", "accelerate") * _acceleration;
		_drivingWheels.ForEach(x => ApplyAcceleration(x, acceleration));
    }

	private void ApplyAcceleration(VehicleWheel3D wheel, float acceleration) {
		var rpm = Mathf.Abs(wheel.GetRpm());
		wheel.EngineForce = acceleration * _maxTorque * (1 - rpm / _maxRPM);
	}

	protected List<VehicleWheel3D> GetDrivingWheels() {
		return new List<VehicleWheel3D>
        {
            GetNode<VehicleWheel3D>("BackLeftWheel"),
            GetNode<VehicleWheel3D>("BackRightWheel")
        };
	}
}
