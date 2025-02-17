using Godot;
using System;

public partial class VFX : Node3D
{
	[Export]
	private GpuParticles3D[] eParticles;

    public override void _Ready()
    {
		PlayEffect();
    }
    public void PlayEffect()
	{
		foreach (var particle in eParticles)
		{
			particle.Emitting = true;
		}
	}
}
