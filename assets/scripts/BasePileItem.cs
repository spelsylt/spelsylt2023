using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BasePileItem : RigidBody3D
{
	private PhysicsCar _car;
	private List<BasePileItem>  _childItems;
	private BasePileItem _parent;
	private Vector3? _originalPosition;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// TODO an item can probably have two parents..
		_childItems = GetChildren().Where(x => x is BasePileItem).Cast<BasePileItem>().ToList();
		_parent = GetParentOrNull<BasePileItem>();
		_originalPosition = Position;
	}
	
	public void setCar(PhysicsCar car) {
		_car = car;
		_childItems.ForEach(x => x.setCar(car));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Freeze == true) {
			var lean = _car.LinearVelocity.Length() * _car.AngularVelocity;
			float sideLean = -lean.Y * 0.01f;
			Rotation = new Vector3(sideLean, 0.0f, 0.0f);
			Position = (Vector3)_originalPosition + new Vector3(0.0f, 0.0f, sideLean);
		}
	}

	public bool IsLeaf() {
		return _childItems.Count() == 0;
	}

	public float GetValue() {
		return Mass;
	}

	public float GetValueOfBranch() {
		if (IsLeaf()) {
			return Mass;
		}
		return Mass + _childItems.Sum(x => x.GetValueOfBranch());
	}

	public void FreezeBranch() {
		Freeze = true;
		_childItems.ForEach(x => x.FreezeBranch());
	}

	public void ReleaseBranch() {
		Freeze = false;
		// TODO launch everything with a little bit of force in a random direction
		_childItems.ForEach(x => x.ReleaseBranch());
		if (_parent != null) {
			_parent.RemoveFromChildren(this);
		}
	}

	public void RemoveFromChildren(BasePileItem child) {
		_childItems.Remove(child);
	}

	public List<BasePileItem> GetLeafs() {
		if (IsLeaf()) {
			return new List<BasePileItem>(){this};
		} else {
			List<BasePileItem> leafs = new List<BasePileItem>();
			foreach(var item in _childItems) {
				leafs.AddRange(item.GetLeafs());
			}
			return leafs;
		}
	}

	public BasePileItem GetParentItem() {
		// TODO support multiple parents?
		return _parent;
	}
}
