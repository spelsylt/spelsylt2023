using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Pile : Node3D
{
	[Export] private PhysicsCar _car;
	[Export] private float _accelerationToScoreFactor = 2.0f;
	[Export] private float _accelerationLimit = 10.0f;
	private List<BasePileItem>  _pileItems;

	private Vector3? _lastVelocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_car == null) {
			_car = GetParent<PhysicsCar>();
			if(_car == null) {
				throw new ArgumentException("No car attached to pile!");
			}
		}

		_pileItems = GetChildren().Where(x => x is BasePileItem).Cast<BasePileItem>().ToList();
		_car.PileStartMass = _pileItems.Sum(x => x.GetValueOfBranch());
		_car.StartMass = _car.Mass;
		_car.Mass += _car.PileStartMass;
		_pileItems.ForEach(x => x.FreezeBranch());
		// TODO can we adjust the car collider to cover all the items for some cheap easy collision?
		
		_pileItems.ForEach(x => x.setCar(_car));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// TODO take angular velocity into account?
		if (_car.IsDisabled()) {
			return;
		}
		var velocity = _car.LinearVelocity;
		if (_lastVelocity != null) {
			float acceleration = ((velocity - (Vector3)_lastVelocity) / (float) delta).Length();
			if (acceleration > _accelerationLimit) {
				GD.Print("Acc: " + acceleration);
				loseItems(acceleration * _accelerationToScoreFactor);
			}
		}
		_lastVelocity = velocity;
	}

	private void loseItems(float score) {
		GD.Print("Losing items for value: " + score);
		if (_pileItems.Count() == 0) {
			return;
		}
		var leaf = getRandomLeaf(_pileItems);
		if (leaf.GetValue() > score) {
			return;
		}
		release(leaf);
		var remaining = score - leaf.GetValue();
		if (remaining <= 0) {
			return;
		} else {
			loseItems(score);
		}
	}

	private void release(BasePileItem item) {
		if (item.GetParentItem() == null) {
			_pileItems.Remove(item);
			var pos = item.GlobalPosition;
			RemoveChild(item);
			GetTree().Root.AddChild(item);
			item.GlobalPosition = pos;
			item.ApplyForce(new Vector3(GD.Randf(), GD.Randf(), GD.Randf()) * 100f * GD.Randf());
		} else {
			item.GetParentItem().RemoveChildItem(item);
		}
		_car.Mass -= item.GetValue();
	}

	private BasePileItem getRandomLeaf(List<BasePileItem> items) {
		var item = items[GD.RandRange(0, items.Count() - 1)];
		if (item.IsLeaf()) {
			return item;
		}
		return getRandomLeaf(item.GetChildItems());
	}

	private BasePileItem GetLoseCandidates(float score, BasePileItem next) {
		if (next.GetValueOfBranch() >= score) {
			return next;
		} else if (next.GetParentItem() != null) {
			return GetLoseCandidates(score, next.GetParentItem());
		} else {
			return next;
		}
	}
	
}
