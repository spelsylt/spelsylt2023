using Godot;
using System;
using System.Linq;

public partial class PhysicsAICar : VehicleBody3D
{
	[Export] private float _speed = 100.0f;
	[Export] private NodePath _PatrolPath;
	[Export] private AudioStreamPlayer3D _EngineSound;
	[Export] private AudioStreamPlayer3D _HornSound;
	float gravity = 9.8f;
	private Vector3[] _patrolPoints;
	private int _patrolIndex = 0;
	private bool _active = true;

	private Vector3 Velocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_PatrolPath != null) {
			_patrolPoints = GetNode<Path3D>(_PatrolPath).Curve.GetBakedPoints();
			Vector3 closest = _patrolPoints[0];
			for (var i = 0; i < _patrolPoints.Count(); i++) {
				var target = _patrolPoints[i];
				var targetFlat = new Vector3(target.X, Position.Y, target.Z);
				var relative = Position - targetFlat;
				if (GlobalTransform.Basis.Z.Dot(relative) < 0) {
					if (Position.DistanceTo(targetFlat) < Position.DistanceTo(new Vector3(closest.X, Position.Y, closest.Z))) {
						closest = target;
						_patrolIndex = i;
					}
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_patrolPoints == null || !_active) {
			return;
		}

		var target = _patrolPoints[_patrolIndex];
		var targetFlat = new Vector3(target.X, Position.Y, target.Z);
		//GD.Print(Position.DistanceTo(targetFlat));
		if (Position.DistanceTo(targetFlat) < 2f) {
			_patrolIndex = _patrolIndex + 1;
			if (_patrolIndex >= _patrolPoints.Count() - 1) {
				_active = false;
				EngineForce = 0;
				return;
			}
			target = _patrolPoints[_patrolIndex];
			targetFlat = new Vector3(target.X, Position.Y, target.Z);
		}
		GD.Print(Position.AngleTo(targetFlat));
		//Steering = (-LinearVelocity.Normalized().AngleTo(targetFlat)) / 10f;
		//GD.Print(Steering);

		var targetVector = targetFlat - Position;
		var fwd = Transform.Basis.Z;
		Steering = fwd.Cross(targetVector.Normalized()).Y;
		GD.Print(Steering);

		EngineForce = 0;
		if (LinearVelocity.Length() < 10) {
			EngineForce = _speed * 100 * (float)delta;
		}
	}

	public void Activate() {
		_active = true;
	}

	public void Disable()  {
		_active = false;
		_EngineSound.Stop();
		EngineForce = 0.0f;
		Brake = 100.0f;
	}

	public void OnCollide(Node3D body) {
		if (body is PhysicsCar) {
			if (!_HornSound.Playing && _active) {
				_HornSound.Play();
			}
			Disable();
			// despawn after a min?
		}
	}
}
