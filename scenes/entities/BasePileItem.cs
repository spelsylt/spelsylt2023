using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BasePileItem : RigidBody3D
{
	private List<BasePileItem>  _childItems;
	private BasePileItem _parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// TODO an item can probably have two parents..
		_childItems = GetChildren().Where(x => x is BasePileItem).Cast<BasePileItem>().ToList();
		_parent = GetParentOrNull<BasePileItem>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
