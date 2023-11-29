using Godot;
using System;

public partial class ScaleUI : Control
{
	[Export] private  Label _weightLabel;
	[Export] private  Label _timeLabel;
	[Export] private  Button _MainMenuButton;
	[Export] private float _weightSpeed = 500.0f;
	[Export] private AudioStreamPlayer _ScaleRiser;
	[Export] private AudioStreamPlayer _AirHorn;
	[Export] private Scale _scale;
	[Export] private PackedScene _mainMenu;
	private PhysicsCar _car;
	private float _totalMass;
	private float _startMass;
	private float _weightcounter = 0;
	private bool _weighing = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		_MainMenuButton.Visible = false;
		if (Global.Instance.Car != null) {
			_car = Global.Instance.Car;
			_totalMass = _car.Mass; // The pile mass is already added to the car!
			_startMass = _car.StartMass;
			_timeLabel.Text = GameMode.formatTime(Global.Instance.GameScore.Time);
		} else {
			// test data
			_totalMass = 1500;
			_startMass = 2000;
			_timeLabel.Text = GameMode.formatTime(100000);
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
			_MainMenuButton.Visible = true;
		}
		_weightLabel.Text = Mathf.Floor(_weightcounter) + " Kg";
		_scale?.SetWeight(_weightcounter);
	}

	public void OnMainMenuButtonPressed() {
		GetTree().ChangeSceneToPacked(_mainMenu);
	}
}
