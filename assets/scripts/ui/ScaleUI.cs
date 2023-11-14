using Godot;
using System;

public partial class ScaleUI : Control
{
	[Export] private  Label _weightLabel;
	[Export] private float _weightSpeed = 500.0f;
	[Export] private AudioStreamPlayer _ScaleRiser;
	[Export] private AudioStreamPlayer _AirHorn;
	private PhysicsCar _car;
	private float _totalMass;
	private float _startMass;
	private float _weightcounter = 0;
	private bool _weighing = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Global.Instance.Car != null) {
			_car = Global.Instance.Car;
			_totalMass = _car.Mass; // The pile mass is already added to the car!
			_startMass = _car.StartMass;
		} else {
			// test data
			_totalMass = 1500;
			_startMass = 2000;
		}
		_ScaleRiser.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_weighing) {
			Weigh((float) delta);
		}
	}

	private void Weigh(float delta) {
		if (_weightcounter < _totalMass) {
			_weightcounter += delta * _weightSpeed;
		} else {
			_weightcounter = _totalMass;
			_ScaleRiser.Stop();
			_AirHorn.Play();
			_weighing = false;
		}
		_weightLabel.Text = Mathf.Floor(_weightcounter) + " Kg";
	}
}
