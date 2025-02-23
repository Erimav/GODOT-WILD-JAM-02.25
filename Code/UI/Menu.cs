using Godot;
using System;

public partial class Menu : Control
{
    /*[ExportCategory("External Dependency")]
    [Export]
    private GameController eGameController;*/

    //[Export]
    private PackedScene eMainScene;
    //[Export]
    private PackedScene eMainMenuScene;

    public override void _Ready()
    {
        eMainScene = ResourceLoader.Load<PackedScene>("Scenes/main.tscn");
        eMainMenuScene = ResourceLoader.Load<PackedScene>("Scenes/main_menu.tscn");
    }

    public void NewGame()
    {
        // Hardcoded condition because we can packed Scene 
        if (GetTree().Root.GetChild(0).Name != "Main")
        {
            GetTree().ChangeSceneToPacked(eMainScene);
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
        GetTree().ChangeSceneToPacked(eMainMenuScene);
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
