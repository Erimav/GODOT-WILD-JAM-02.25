using Godot;
using System;

public partial class Projectile : Node3D
{
    [Export]
    private float eSpeed = 10;

    private bool isFired;
    private Vector3 mLastPostition;

    public Target Target { get; set; }

    public override void _Process(double delta)
    {
        if (Target is not null)
        {
            var direction = Target.TargetPosition - GlobalPosition;
            direction.Y = 0;
            var yaw = Mathf.Atan2(direction.X, direction.Z);
            Rotation = new Vector3(0, yaw, 0);
        }
    }

    public void Shoot()
    {
        if (Target is null)
        {
            GD.PrintErr("Trying to fire a projectile with no target");
            return;
        }

        isFired = true;
        mLastPostition = GlobalPosition;

        var distance = GlobalPosition.DistanceTo(Target.TargetPosition);
        var travelTime = distance / eSpeed;
        var tween = CreateTween().SetParallel();
        tween.TweenProperty(this, "global_position:y", Target.TargetPosition.Y + 0.5f, travelTime)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
        tween.TweenMethod(Callable.From<float>(d =>
        {
            if (Target is null)
            {
                QueueFree();
            }
            var flatDirection = Target.TargetPosition - GlobalPosition;
            flatDirection.Y = 0;
            flatDirection = flatDirection.Normalized();
            GlobalPosition = Target.TargetPosition - flatDirection * d + Vector3.Up * GlobalPosition.Y;
            var heightDirection = mLastPostition - GlobalPosition;
            mLastPostition = GlobalPosition;
            if (heightDirection != Vector3.Zero)
            {
                LookAt(GlobalPosition + heightDirection);
            }
        }), distance, 0f, travelTime);
        tween.TweenCallback(Callable.From(() =>
        {
            if (Target is null)
            {
                QueueFree();
            }
            if (IsInstanceValid(Target))
            {
                Target.TakeHit(1);
            }
            QueueFree();
        })).SetDelay(travelTime);
    }
}
