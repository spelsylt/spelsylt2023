using Godot;
using System;

public partial class VolumeSliderUI : HSlider
{
	[Export] private string _busName;
	private int _busIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_busIndex = AudioServer.GetBusIndex(_busName);
		ValueChanged += OnValueChanged;
		Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_busIndex));
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	private void OnValueChanged(double value)
    {
        AudioServer.SetBusVolumeDb(_busIndex, (float) Mathf.LinearToDb(value));
    }
}
