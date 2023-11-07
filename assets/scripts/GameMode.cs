using Godot;
using System;

public partial class GameMode : Node
{
	private float _startTimer = 3.0f;
	private float _gameTimer = 0.0f;
	private bool _gameRunning = false;
	private bool _countDown = true;
	[Export] private PhysicsCar _car;
	[Export] private Label _CountdownText;
	[Export] private Label _TimeText;

	[Export] private Timer _GoTextTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_car.Freeze = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_gameRunning) {
			GameRunning((float) delta);
		} else if(_countDown) {
			_startTimer -= (float) delta;
			if (_startTimer <= 0.0f) {
				_gameRunning = true;
				_car.Freeze = false;
				_countDown = false;
				_CountdownText.Text = "GO"; // TODO hide after some time
				_GoTextTimer.Start();
			} else {
				int count = (int) Mathf.Ceil(_startTimer);
				_CountdownText.Text = count.ToString();
			}
		}
	}

	private void GameRunning(float delta) {
		_gameTimer += delta;
		_TimeText.Text = formatTime(_gameTimer);
	}

	public void EndGame() {
		_gameRunning = false;
		// TODO handle end of game stuff
	}

	public void HideGoText() {
		_CountdownText.Hide();
	}

	private String formatTime(float time) {
		var milliseconds  = time * 1000;
		TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
		return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", 
							t.Hours, 
							t.Minutes, 
							t.Seconds, 
							t.Milliseconds);
	}
}
