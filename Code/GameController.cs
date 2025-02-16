using Godot;
using System;

public partial class GameController : Node
{
    public enum GameState
    {
        Prepare,
        Wave
    }

    [ExportCategory("External Exports")]
    [Export]
    private MobController eMobController;

    [Export]
    private MapObject eMapObject;

    [ExportCategory("Game Parameters")]
    [Export]
    private int eSpawnNumber;

    public bool CanSetToWave()
    {
        if (GameManager.GetInstance().GetGameState() == GameManager.GameState.Wave) 
            return false;
        var paths = eMapObject.FindAllPaths();
        if (paths.Count == 0)
            return false;
        return true;
    }

    public bool TryToToSetWave()
    {
        if (CanSetToWave())
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.Wave);
            return true;
        }
        return false;
    }

    public bool TryToSetPrepare()
    {

        GameManager.GetInstance().ChangeState(GameManager.GameState.Prepare);
        return true;
    }

    public void StartWave()
    {
        GD.Print("Game Controller: Start Wave");
        eMobController.StartSpawn(eSpawnNumber);
    }


}
