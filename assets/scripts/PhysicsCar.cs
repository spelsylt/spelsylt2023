using Godot;
using System;
using System.Collections.Generic;

public partial class PhysicsCar : VehicleBody3D
{
	[Export] private float _acceleration = 10f;
	[Export] private float _maxRPM = 500;
	[Export] private float _maxTorque = 50;
	[Export] private float _steeringSpeed = 5.0f;
	[Export] private PlayerCamera _playerCamera;
	[Export] private float _brakeForce = 50.0f;
	[Export] private float _handbrakeForce = 100.0f;

	public float RPM {private set; get;} = 0.0f;

	private List<VehicleWheel3D> _drivingWheels;
	private List<VehicleWheel3D> _handbrakeWheels;

    public override void _Ready()
    {
        base._Ready();
		_playerCamera.target = this;
		_drivingWheels = GetDrivingWheels();
		_handbrakeWheels = GetHandbrakeWheels();
    }

    public override void _PhysicsProcess(double delta)
    {
        Steering = Mathf.Lerp(Steering, Input.GetAxis("steer_right", "steer_left") * 0.4f, _steeringSpeed * (float)delta);
		var acceleration = Input.GetAxis("brake_reverse", "accelerate") * _acceleration;
		_drivingWheels.ForEach(x => ApplyAcceleration(x, acceleration));

		if (Input.IsActionPressed("handbrake")) {
			_handbrakeWheels.ForEach(wheel => wheel.Brake = _handbrakeForce);
		} else {
			_handbrakeWheels.ForEach(wheel => wheel.Brake = 0.0f);
		}
    }

	private void ApplyAcceleration(VehicleWheel3D wheel, float acceleration) {
		var rpm = Mathf.Abs(wheel.GetRpm());
		if ((acceleration < 0 && wheel.GetRpm() > 0) || acceleration > 0 && wheel.GetRpm() < 0) {
			wheel.Brake = _brakeForce;
			wheel.EngineForce = 0f;
		} else {
			wheel.Brake = 0f;
			wheel.EngineForce = acceleration * _maxTorque * (1 - rpm / _maxRPM);
		}
		RPM = rpm; // this gives the wheel rpm, for now until we simulate engine rpms.
	}

	protected List<VehicleWheel3D> GetDrivingWheels() {
		return new List<VehicleWheel3D>
        {
            GetNode<VehicleWheel3D>("BackLeftWheel"),
            GetNode<VehicleWheel3D>("BackRightWheel")
        };
	}

	protected List<VehicleWheel3D> GetHandbrakeWheels() {
		return new List<VehicleWheel3D>
        {
            GetNode<VehicleWheel3D>("FrontLeftWheel"),
            GetNode<VehicleWheel3D>("FrontRightWheel")
        };
	}
}
