using Godot;
using System;

public partial class Menu : Node
{
    [Export]
    private PackedScene eMainScene;
    [Export]
    private PackedScene mMainMenuScene;

    public void NewGame()
    {
        GetTree().ChangeSceneToPacked(eMainScene);
    }

    public void MainMenu()
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
