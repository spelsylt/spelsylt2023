using Godot;
using System;

public partial class CarModelInfo : Node3D
{
	[Export] public Node3D FrontWheelLeft;
	[Export] public Node3D FrontWheelRight;
	[Export] public Node3D BackWheelLeft;
	[Export] public Node3D BackWheelRight;
}
