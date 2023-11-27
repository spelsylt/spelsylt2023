using Godot;
using System;
using System.Linq;

public partial class AICar : CharacterBody3D
{
	[Export] private float _speed = 100.0f;
	[Export] private NodePath _PatrolPath;
	[Export] private AudioStreamPlayer3D _EngineSound;
	[Export] private AudioStreamPlayer3D _HornSound;
	[Export] private RayCast3D _raycast;
	float gravity = 1.0f;
	private Vector3[] _patrolPoints;
	private int _patrolIndex = 0;
	private bool _active = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_PatrolPath != null) {
			_patrolPoints = GetNode<Path3D>(_PatrolPath).Curve.GetBakedPoints();
			Vector3 closest = _patrolPoints[0];
			for (var i = 0; i < _patrolPoints.Length; i++) {
				var target = _patrolPoints[i];
				var targetFlat = new Vector3(target.X, Position.Y, target.Z);
				if (Position.DistanceTo(targetFlat) < Position.DistanceTo(new Vector3(closest.X, Position.Y, closest.Z))) {
					closest = target;
					_patrolIndex = i;
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
		if (Position.DistanceTo(targetFlat) < 2f) {
			_patrolIndex = Mathf.Wrap(_patrolIndex + 1, 0, _patrolPoints.Count());
			target = _patrolPoints[_patrolIndex];
			targetFlat = new Vector3(target.X, Position.Y, target.Z);
		}
		
		Velocity = (targetFlat - Position).Normalized() * _speed * 3.6f * (float) delta;
		Velocity += Vector3.Down * gravity;
		LookAt(targetFlat, Vector3.Up);
		MoveAndSlide();

		var n = _raycast.GetCollisionNormal();
		var xform = Align(GlobalTransform, n);
		//GlobalTransform = xform;
	}

	public void Activate() {
		_active = true;
	}

	public void Disable()  {
		_active = false;
		_EngineSound.Stop();
	}

	public void OnCollide(Node3D body) {
		if (body is PhysicsCar) {
			Disable();
			_HornSound.Play();
			// despawn after a min?
		}
	}

	private Transform3D Align(Transform3D x, Vector3 newY) {
		x.Basis.Y = newY;
		x.Basis.X = -x.Basis.Z.Cross(newY);
		x.Basis = x.Basis.Orthonormalized();
		return x;
	}
}
