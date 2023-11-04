using Godot;

public partial class car : CharacterBody3D
{
    [Export] private float _gravity = -20.0f;
    [Export] private float _wheelBase = 0.6f; // distance between front/rear axels (TODO calculate?)
    [Export] private float _steeringLimit = 10.0f; // front wheel max turning angle in degrees
    [Export] private float _enginePower = 6.0f;
    [Export] private float _braking = -9.0f;
    [Export] private float _handbrakeForce = -5.0f;
    [Export] private float _friction = -2.0f;
    [Export] private float _drag = -2.0f;
    [Export] private float _maxSpeedreverse = 3.0f;
    [Export] private PlayerCamera _playerCamera;
    [Export] private Node3D _cameraPos;
    [Export] private float _slipSpeed = 9.0f;
    [Export] private float _tracationSlow = 0.75f;
    [Export] private float _tractionFast = 0.02f;
    [Export] private float _tranctionHandbrake = 0.01f;

    private bool _drifting = false;
    private bool _handbraking = false;

    private Vector3 _acceleration = Vector3.Zero;
    private Vector3 _velocity = Vector3.Zero;
    private float _steerAngle = 0.0f;
    [Export] private CarModelInfo _model;

    public override void _Ready()
    {
        base._Ready();
        _playerCamera.target = _cameraPos;
    }

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

        if (!_drifting && _velocity.Length() > _slipSpeed) {
            _drifting = true;
        }
        if (_drifting && _velocity.Length() < _slipSpeed && _steerAngle == 0) {
            _drifting = false;
        }
        var traction = _drifting ? _tractionFast : _tracationSlow;
        if (_handbraking) {
            traction = _tranctionHandbrake;
        }

        var d = newHeading.Dot(_velocity.Normalized());
        var targetVelocity = Vector3.Zero;
        bool forward = true;
        if (d > 0)
        {
            targetVelocity = newHeading * _velocity.Length();
            forward = true;
        } 
        else if (d < 0) 
        {
            forward = false;
            targetVelocity = -newHeading * (_velocity.Length() < _maxSpeedreverse ? _velocity.Length() : _maxSpeedreverse);
        }
        _velocity = _velocity.Lerp(targetVelocity, traction);
        SpinWheels(delta, forward, targetVelocity);
        LookAt(Transform.Origin + newHeading, Transform.Basis.Y);
    }

    private void GetInput() 
    {
        var turn = Input.GetActionStrength("steer_left");
        turn -= Input.GetActionStrength("steer_right");
        _steerAngle = turn * Mathf.DegToRad(_steeringLimit);
        _model.FrontWheelRight.Rotation = new Vector3(_model.FrontWheelRight.Rotation.X, _steerAngle*2, 0f);
        _model.FrontWheelLeft.Rotation = new Vector3(_model.FrontWheelLeft.Rotation.X, _steerAngle*2, 0f);
        _acceleration = Vector3.Zero;
        _handbraking = false;
        var accChange = 0.0f;
        if (Input.IsActionPressed("accelerate"))
        {
            accChange = _enginePower;
        }
        if (Input.IsActionPressed("brake_reverse"))
        {
            accChange = _braking;
        }
        _acceleration = -Transform.Basis.Z * accChange;
        if (Input.IsActionPressed("handbrake")) {
            _acceleration = Vector3.Zero;
            _drifting = true;
            _handbraking = true;
            _acceleration = _velocity.Normalized() * _handbrakeForce;
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

    private void SpinWheels(float delta, bool forward, Vector3 targetVelocity) {
        var spinSpeed = targetVelocity.Length();
        if (forward) {
            spinSpeed = -spinSpeed;
        }
        spinWheel(_model.FrontWheelLeft, spinSpeed, delta);
        spinWheel(_model.FrontWheelRight, spinSpeed, delta);
        spinWheel(_model.BackWheelLeft, spinSpeed, delta);
        spinWheel(_model.BackWheelRight, spinSpeed, delta);
    }

    private void spinWheel(Node3D wheel, float speed, float delta) {
        wheel.Rotation = new Vector3(wheel.Rotation.X + (speed * delta), wheel.Rotation.Y, wheel.Rotation.Z);
    }
}
