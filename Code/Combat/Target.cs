using Godot;
using System;
using System.Collections.Generic;

public partial class Target : Node
{
	[Signal]
	public delegate void HitEventHandler(int damage); 
	public static List<Target> AllTargets = new List<Target>();

	public Vector3 TargetPosition => GetParent<Node3D>().GlobalPosition;

	public override void _Ready()
	{
		AllTargets.Add(this);
	}

	public void TakeHit(int damage)
	{
		GD.Print("Ouch!");
		EmitSignal(SignalName.Hit, damage);
	}
}
