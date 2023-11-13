using Godot;
using System;
using System.Linq;

public partial class AICar : CharacterBody3D
{
	[Export] private float _speed = 100.0f;
	[Export] private NodePath _PatrolPath;
	[Export] private AudioStreamPlayer3D _EngineSound;
	private Vector3[] _patrolPoints;
	private int _patrolIndex = 0;
	private bool _active = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_PatrolPath != null) {
			_patrolPoints = GetNode<Path3D>(_PatrolPath).Curve.GetBakedPoints();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_patrolPoints == null || !_active) {
			return;
		}

		var target = _patrolPoints[_patrolIndex];
		if (Position.DistanceTo(target) < 4f) {
			_patrolIndex = Mathf.Wrap(_patrolIndex + 1, 0, _patrolPoints.Count());
			target = _patrolPoints[_patrolIndex];
		}
		var targetFlat = new Vector3(target.X, Position.Y, target.Z);
		Velocity = (targetFlat - Position).Normalized() * _speed * 3.6f * (float) delta;
		LookAt(targetFlat, Vector3.Up);
		MoveAndSlide();
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
			// despawn after a min?
			// todo honk the horn
		}
	}
}
