using Godot;
using System;

public partial class Mob : Node3D
{
	[Signal]
	public delegate void OnMobFinishedPathEventHandler(Mob mob);
	[Signal]
	public delegate void OnMobDeadPathEventHandler(Mob mob);

	[Export]
	private Target eTarget;

	[Export]
	private float eMaxHP;
	[Export]
	private float eMoveSpeed;

	// PRIVATE
	private float mCurHP;
	private PathFollow3D mPath;

	// Public
	public void SetPath(PathFollow3D path)
	{
		mPath = path;
		path.AddChild(this);
	}

    public override void _Ready()
    {
		mCurHP = eMaxHP;
        base._Ready();
    }

    public override void _Process(double delta)
    {
		if (mPath != null)
		{
			mPath.Progress += eMoveSpeed * (float)delta;
			if (mPath.ProgressRatio >= 1.0f)
			{
				EmitSignal("OnMobFinishedPath", this);
				Destroy();
			}
		}
		if (mCurHP <= 0.0f)
		{
			EmitSignal("OnMobDeadPath", this);
            Destroy();
        }
		base._Process(delta);
    }

	public void Destroy()
	{
		eTarget.Destroy();
		mPath.QueueFree();
	}

	public void TakeHit(int damage)
	{
		mCurHP -= damage;
	}


}
