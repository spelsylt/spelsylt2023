using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PhysicsCar : VehicleBody3D
{
	[Export] private float _acceleration = 10f;
	[Export] private float _maxRPM = 500;
	[Export] private float _RPMGearChange = 3000;
	[Export] private float _maxEngineForce = 50;
	[Export] private Curve _powerCruve = null;
	[Export] private float _maxTorque = 50;
	[Export] private float _steeringSpeed = 5.0f;
	[Export] private PlayerCamera _playerCamera;
	[Export] private float _brakeForce = 50.0f;
	[Export] private float _handbrakeForce = 100.0f;

	private float _throttle = 0.0f;

	public float RPM {private set; get;} = 0.0f;

	private List<VehicleWheel3D> _drivingWheels;
	private List<VehicleWheel3D> _handbrakeWheels;

	private float _wheelCircumference = 0f;
	[Export] private float _finalDriveRatio = 5.308f;
	[Export] private float[] _GearRatios = new float[]{3.909f, 2.056f, 1.344f, 0.978f, 0.837f};
	[Export] private float _reverseGearRatio = -3.727f;
	private int _currentGear = 1;
	private float _clutchPosition = 1.0f;
	private float _gearShiftTime = 0.3f;
	private float _gearShiftTimer = 0.0f;

	public float StartMass = 0;
	public float PileStartMass = 0;

	[Export] private bool _disabled = false;

	public int Gear {get {return _currentGear;} private set {}}

    public override void _Ready()
    {
        base._Ready();
		_playerCamera._car = this;
		_drivingWheels = GetDrivingWheels();
		_handbrakeWheels = GetHandbrakeWheels();

		// Does not support different sized driving wheels. Just dont.. do that.
		_wheelCircumference = _drivingWheels[0].WheelRadius * 2.0f * Mathf.Pi;
    }

    public override void _PhysicsProcess(double delta)
    {
		if (_disabled) {
			return;
		}
        Steering = Mathf.Lerp(Steering, Input.GetAxis("steer_right", "steer_left") * 0.4f, _steeringSpeed * (float)delta);
		var inputThrottle = Input.GetAxis("brake_reverse", "accelerate");

		CalculateRPM(inputThrottle, (float) delta);
		AutoGear(inputThrottle, (float) delta);
		CalculateAndApplyEngineForce();

		if (Input.IsActionPressed("handbrake")) {
			_handbrakeWheels.ForEach(wheel => wheel.Brake = _handbrakeForce);
		} else {
			_handbrakeWheels.ForEach(wheel => wheel.Brake = 0.0f);
		}
    }

	private void AutoGear(float inputThrottle, float delta) {
		_throttle = Mathf.Abs(inputThrottle) * _acceleration;
		var currentGear = _currentGear;
		if (_gearShiftTimer > 0) {
			_gearShiftTimer = Mathf.Max(0.0f, _gearShiftTimer - delta);
			_clutchPosition = Mathf.Lerp(0.0f, 1.0f, 1.0f - _gearShiftTimer);
		} else {
			if (LinearVelocity.Length() < 0.1f) {
				if (inputThrottle < 0 && RPM >= 1000) {
					_currentGear = -1;
				} else if (inputThrottle > 0 && RPM >= 1000) {
					_currentGear = 1;
				} else if (inputThrottle == 0) {
					_currentGear = 0;
				}
			} else {
				var speed = LinearVelocity.Length() * 3.6;
				SwitchGear(speed);
			}
			if (_currentGear != currentGear) {
				_gearShiftTimer = _gearShiftTime;
				_clutchPosition = 0.0f;
			}
		}
		if ((_currentGear < 0 && inputThrottle > 0) || (_currentGear > 0 && inputThrottle < 0)) {
			Brake = _brakeForce;
			_throttle = 0.0f;
		} else {
			Brake = 0.0f;
		}
	}

	private void SwitchGear(double speed) {
		if (_currentGear != -1) {
			if (_currentGear == 1) {
				if (speed >= 15) {
					_currentGear++;
				}
			} else if (_currentGear == 2) {
				if (speed <= 10) {
					_currentGear--;
				} else if (speed >= 30) {
					_currentGear++;
				}
			} else if (_currentGear == 3) {
				if (speed <= 25) {
					_currentGear--;
				} else if (speed >= 50) {
					_currentGear++;
				}
			} else if (_currentGear == 4) {
				if (speed <= 45) {
					_currentGear--;
				} else if (speed >= 70) {
					_currentGear++;
				}
			} else if (_currentGear == 5) {
				if (speed <= 65) {
					_currentGear--;
				}
			}
		}
	}

	private void CalculateRPM(float throttle, float delta) {
		var speed = LinearVelocity.Length();
		var wheelRotSpeed = 60f * speed / _wheelCircumference;
		var driveShaftSpeed = wheelRotSpeed * _finalDriveRatio;

		if (speed < 0.1f) {
			if (throttle > 0.1f || throttle < -0.1f) {
				RPM += delta * Mathf.Abs(throttle) * 1000f;
			} else {
				RPM -= delta * 1000f;
				if (RPM < 500) {
					RPM = 500;
				}
			}
			if (RPM > _maxRPM) {
				RPM = _maxRPM;
			}
			return;
		}
		
		if (_currentGear == -1) {
			RPM = driveShaftSpeed * -_reverseGearRatio;
		} else if (_currentGear <= _GearRatios.Count() && _currentGear > 0) {
			RPM = driveShaftSpeed * _GearRatios[_currentGear - 1];
		} else {
			RPM = 0.0f;
		}
		if (RPM < 500) {
			RPM = 500;
		}
	}

	private void CalculateAndApplyEngineForce() {
		var rpmFactor = Mathf.Clamp(RPM / _maxRPM, 0.0f, 1.0f);
		var powerFactor = _powerCruve.Sample(rpmFactor);

		if (_currentGear == -1) {
			EngineForce = _clutchPosition * _throttle * powerFactor * _reverseGearRatio * _finalDriveRatio * _maxEngineForce;
		} else if (_currentGear > 0 && _currentGear <= _GearRatios.Count()) {
			EngineForce =  _clutchPosition * _throttle * powerFactor * _GearRatios[_currentGear - 1] * _finalDriveRatio * _maxEngineForce;
		} else {
			EngineForce = 0.0f;
		}
	}

	public void Disable() {
		Freeze = false;
		_disabled = true;
	}

	public bool IsDisabled() {
		return _disabled;
	}

	public float GetIsAnyWheelSkidding() {
		return _drivingWheels.Max(x => x.GetSkidinfo());
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
