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
				loseItem(acceleration * _accelerationToScoreFactor);
			}
		}
		_lastVelocity = velocity;
	}

	private void loseItem(float score) {
		GD.Print("Lose item for score: ", score);
		List<BasePileItem> leafs = new List<BasePileItem>();
		foreach(var item in _pileItems) {
			leafs.AddRange(item.GetLeafs());
		}
		List<BasePileItem> candidates = new List<BasePileItem>();
		foreach(var leaf in leafs) {
			var candidate = GetLoseCandidates(score, leaf);
			if (candidate != null) {
				candidates.Add(candidate);
			}
		}
		if (candidates.Count() == 0) {
			GD.Print("No items elegible to lose for score: ", score);
			return;
		} else {
			GD.Print("Candidates for loss: ", candidates.Count());
			int index = (int)MathF.Ceiling(GD.RandRange(0, candidates.Count() - 1));
			var itemBranchToLose = candidates[index];
			var valLoss = itemBranchToLose.GetValueOfBranch();
			itemBranchToLose.ReleaseBranch();
			if (itemBranchToLose.GetParentItem() == null) {
				_pileItems.Remove(itemBranchToLose);
				var pos = GlobalPosition;
				RemoveChild(itemBranchToLose);
				GetTree().Root.AddChild(itemBranchToLose);
				itemBranchToLose.GlobalPosition = pos;
			}
			_car.Mass -= valLoss;
		}
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
