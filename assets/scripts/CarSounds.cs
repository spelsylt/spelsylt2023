using Godot;
using System;

public partial class CarSounds : Node3D
{
	[Export] private PhysicsCar _car;
	[Export] private float _IdleRPM = 500f;
	[Export] private float _MaxRPM = 7500f;
	[Export] private float _skidSoundThreshold = 0.2f;
	private AudioStreamPlayer3D _enginePlayer;
	private AudioStreamPlayer3D _skidSoundPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_enginePlayer = GetChild<AudioStreamPlayer3D>(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_car.IsDisabled()) {
			_enginePlayer?.Stop();
			_skidSoundPlayer?.Stop();
		} else {
			EngineSound();
			TireSounds();
		}
	}

	private void EngineSound() {
		var rpm = _car.RPM;
		var t = Mathf.Lerp(0.8f, 2.5f, rpm / _MaxRPM);
		_enginePlayer.PitchScale = t;
	}

	private void TireSounds() {
		var skid = _car.GetIsAnyWheelSkidding();
		if (skid <= _skidSoundThreshold) {
			//_skidSoundPlayer.Play();
		}

	}
}
