using Godot;
using System;
using System.Collections.Generic;

public partial class Target : Node
{
	[Signal]
	public delegate void HitEventHandler(int damage);
	public static List<Target> AllTargets = new List<Target>();

	public Vector3 TargetPosition {
		get
		{
			if (IsInstanceValid(this) && IsInstanceValid(GetParent<Node3D>()))
			{
				Node3D parent = GetParent<Node3D>();
				if (parent is null) return mLastPosition;
				mLastPosition = parent.GlobalPosition;
				return parent.GlobalPosition;
			}
			return mLastPosition;
		}
	}

	private Vector3 mLastPosition = Vector3.Zero;

	public override void _Ready()
	{
		AllTargets.Add(this);
	}

	public void Destroy()
	{
		AllTargets.Remove(this);
		QueueFree();
	}

	public void TakeHit(int damage)
	{
		GD.Print("Ouch!");
		EmitSignal(SignalName.Hit, damage);
	}

}
