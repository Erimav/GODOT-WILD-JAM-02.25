using Godot;
using System;

public partial class Tower : Node3D
{
    [Export]
    private Node3D eProjectileSpawnPoint;
    [Export]
    private PackedScene eProjectilePrefab;
    [Export]
    private float eDistance;
    [Export]
    private float ePrice;


    private Timer mShootTimer;
    private Timer mReloadTimer;

    private Projectile mProjectileInChamber;
    private Target mTarget;


    public float Price
    {
        get => ePrice;
    }

    public Target Target
    {
        get => mTarget;
        private set
        {
            mTarget = value;
            if (mProjectileInChamber is not null)
            {
                mProjectileInChamber.Target = value;
            }
        }
    }


    public override void _Ready()
    {
        LoadProjectile();

        mReloadTimer = GetNode<Timer>("ReloadTimer");
        mShootTimer = GetNode<Timer>("ShootTimer");
        mReloadTimer.Timeout += LoadProjectile;
        mShootTimer.Timeout += Shoot;
    }


    public override void _Process(double delta)
    {
        
        if (Target is null)
        {
            if (FindClosestExposedTarget() is Target target)
            {
                Target = target;
                mShootTimer.Start();
            }
        }
        else if (!IsInstanceValid(Target) || (!IsTargetExposed(Target) || GlobalPosition.DistanceTo(Target.TargetPosition) > eDistance))
        {
            GD.Print("Is Target Valid: " + IsInstanceValid(Target));
            var newTarget = FindClosestExposedTarget();
            if (newTarget is not null)
            {
                Target = newTarget;
            }
            else
            {
                Target = null;
                mShootTimer.Stop();
            }
        }
    }


    private void LoadProjectile()
    {
        if (mProjectileInChamber is not null)
        {
            return;
        }

        var projectile = eProjectilePrefab.Instantiate<Projectile>();
        GetTree().Root.CallDeferred("add_child", projectile);
        projectile.Position = eProjectileSpawnPoint.GlobalPosition;
        projectile.Target = mTarget;
        mProjectileInChamber = projectile;
    }

    private void Shoot()
    {
        if (Target is null)
        {
            GD.PrintErr("Trying to shoot a null target");
            return;
        }

        if (mProjectileInChamber is null)
        {
            return;
        }

        mProjectileInChamber.Shoot();
        mProjectileInChamber = null;
        mReloadTimer.Start();
    }

    private Target FindClosestExposedTarget()
    {
        Target closest = null;
        float closestDistance = float.MaxValue;
        float maxDistanceSquared = eDistance * eDistance;

        foreach (var target in Target.AllTargets)
        {
            if (IsTargetExposed(target))
            {
                var distanceSquared = GlobalPosition.DistanceSquaredTo(target.TargetPosition);
                if (distanceSquared <= maxDistanceSquared)
                {
                    closest = target;
                    closestDistance = distanceSquared;
                }
            }
        }

        return closest;
    }

    private bool IsTargetExposed(Target target)
    {
        return true;
    }
}
