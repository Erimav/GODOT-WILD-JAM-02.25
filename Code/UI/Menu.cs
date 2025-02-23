using Godot;
using System;

public partial class Menu : Control
{
    /*[ExportCategory("External Dependency")]
    [Export]
    private GameController eGameController;*/

    public void NewGame()
    {
        // Hardcoded condition because we can packed Scene 
        if (GetTree().Root.GetChild(0).Name != "Main")
        {
            GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
        }
        else
        {
            GetTree().ReloadCurrentScene();
            //GameController controller = GetTree().Root.GetNode<GameController>("Main/GameController");
            //controller.StartGame();
        }
    }

    public void MainMenu()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
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
