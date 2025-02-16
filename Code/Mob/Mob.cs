using Godot;
using System;

public partial class Mob : Node3D
{
	[Signal]
	public delegate void OnMobFinishedPathEventHandler(Mob mob);
	[Signal]
	public delegate void OnMobDeadPathEventHandler(Mob mob);

	[Export]
	private float mMaxHP;
	[Export]
	private float mMoveSpeed;

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
		mCurHP = mMaxHP;
        base._Ready();
    }

    public override void _Process(double delta)
    {
		if (mPath != null)
		{
			mPath.Progress += mMoveSpeed * (float)delta;
			if (mPath.ProgressRatio >= 1.0f)
			{
				EmitSignal("OnMobFinishedPath", this);
				mPath.QueueFree();
			}
		}
		if (mCurHP <= 0.0f)
		{
			EmitSignal("OnMobDeadPath", this);
			mPath.QueueFree();
		}
		base._Process(delta);
    }


}
