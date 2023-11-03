using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class car : CharacterBody3D
{
    [Export] private float _gravity = -20.0f;
    [Export] private float _wheelBase = 0.6f; // distance between front/rear axels (TODO calculate?)
    [Export] private float _steeringLimit = 10.0f; // front wheel max turning angle in degrees
    [Export] private float _enginePower = 6.0f;
    [Export] private float _braking = -9.0f;
    [Export] private float _friction = -2.0f;
    [Export] private float _drag = -2.0f;
    [Export] private float _maxSpeedreverse = 3.0f;

    private Vector3 _acceleration = Vector3.Zero;
    private Vector3 _velocity = Vector3.Zero;
    private float _steerAngle = 0.0f;
    [Export] private CarModelInfo _model;

    public override void _PhysicsProcess(double delta)
    {
        var deltaF = (float) delta;
        if (IsOnFloor()) 
        {
            GetInput();
            ApplyFriction(deltaF);
            CalcSteering(deltaF);
        }
        _acceleration.Y = _gravity;
        _velocity += _acceleration * deltaF;
        ApplyVelocity();
    }

    private void ApplyFriction(float delta) 
    {
        if (_velocity.Length() < 0.2f && _acceleration.Length() == 0)
        {
            _velocity.X = 0;
            _velocity.Y = 0;
        }
        var frictionForce = _velocity * _friction * delta;
        var dragForce = _velocity * _velocity.Length() * _drag * delta;
        _acceleration += dragForce + frictionForce;
    }

    private void CalcSteering(float delta) 
    {
        var rearWheel = Transform.Origin + Transform.Basis.Z * _wheelBase / 2.0f;
        var frontWheel = Transform.Origin - Transform.Basis.Z * _wheelBase / 2.0f;
        rearWheel += _velocity * delta;
        frontWheel += _velocity.Rotated(Transform.Basis.Y, _steerAngle) * delta;
        var newHeading = rearWheel.DirectionTo(frontWheel);
        var d = newHeading.Dot(_velocity.Normalized());
        if (d > 0)
        {
            _velocity = newHeading * _velocity.Length();
        }
        if (d < 0) 
        {
            _velocity = -newHeading * (_velocity.Length() < _maxSpeedreverse ? _velocity.Length() : _maxSpeedreverse);
        }
        LookAt(Transform.Origin + newHeading, Transform.Basis.Y);
    }

    private void GetInput() 
    {
        var turn = Input.GetActionStrength("steer_left");
        turn -= Input.GetActionStrength("steer_right");
        _steerAngle = turn * Mathf.DegToRad(_steeringLimit);
        _model.FrontWheelRight.Rotation = new Vector3(0f, _steerAngle*2, 0f);
        _model.FrontWheelLeft.Rotation = new Vector3(0f, _steerAngle*2, 0f);
        _acceleration = Vector3.Zero;
        if (Input.IsActionPressed("accelerate"))
        {
            _acceleration = -Transform.Basis.Z * _enginePower;
        }
        if (Input.IsActionPressed("brake_reverse"))
        {
            _acceleration = -Transform.Basis.Z * _braking;
        }
    }

    private void ApplyVelocity() 
    {
        Velocity = _velocity;
        FloorStopOnSlope = true;
        UpDirection = Vector3.Up;
        FloorSnapLength = 1.0f;
        MoveAndSlide();
        _velocity = Velocity;
    }
}
