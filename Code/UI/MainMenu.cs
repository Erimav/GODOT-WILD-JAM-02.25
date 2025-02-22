using Godot;
using System;

public partial class MainMenu : Node
{
    [Export]
    private PackedScene eMainScene;

    public void NewGame()
    {
        GetTree().ChangeSceneToPacked(eMainScene);
    }

    public void Exit()
    {
        GetTree().Quit();
    }

    public void ChangeVolume(float volume, int index)
    {
        AudioServer.SetBusVolumeDb(index, Mathf.LinearToDb(volume));
    }
}
