using Godot;
using System;

public partial class Mob : Node3D
{
	[Signal]
	public delegate void OnMobFinishedPathEventHandler(Mob mob);
	[Signal]
	public delegate void OnMobDeadPathEventHandler(Mob mob);

	[ExportCategory("NodeDependencies")]
	[Export]
	private Target eTarget;
	[Export]
	private AnimationPlayer eAnimationController;

	[ExportCategory("ExportParameters")]
	[Export]
	private float eMaxHP;
	[Export]
	private float eMoveSpeed;

	[Export]
	private AudioStream mDyingSound;

	// PRIVATE
	private float mCurHP;
	private PathFollow3D mPath;
	private bool mDead;

	// Public
	public bool IsDead
	{
		get => mDead;
	}
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
		if (!IsDead)
		{
            if (mPath != null)
            {
                mPath.Progress += eMoveSpeed * (float)delta;
                eAnimationController.CurrentAnimation = "Walking";
                eAnimationController.Play();
                if (mPath.ProgressRatio >= 1.0f)
                {
                    EmitSignal("OnMobFinishedPath", this);
                    eTarget.Destroy();
                    Destroy();
                }
            }
            if (mCurHP <= 0.0f)
            {
                EmitSignal("OnMobDeadPath", this);
				AudioManager.Instance.PlayEnemySound(mDyingSound);
                eAnimationController.CurrentAnimation = "Dying";
				mDead = true;
                eTarget.Destroy();
            }
        } 
		
		base._Process(delta);
    }

	public void OnAnimationEnd(StringName animationName)
	{
		GD.Print("Mob. Animation End");
		if (animationName == "Dying")
		{
			Destroy();
		}
	}

	public void Destroy()
	{
		mPath.QueueFree();
	}

	public void TakeHit(int damage)
	{
		mCurHP -= damage;
	}


}
