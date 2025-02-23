using Godot;
using System;

public partial class SoundVolumeSlider : HSlider
{
    [Export]
    private int eBusIndex = 1;

    public override void _Ready()
    {
        MaxValue = 1;
        Step = 0.01f;
        Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(eBusIndex));
        ValueChanged += ChangeVolume;
    }

    public void ChangeVolume(double volume)
    {
        AudioServer.SetBusVolumeDb(eBusIndex, Mathf.LinearToDb((float)volume));
    }
}
