using Godot;
using System;

public partial class GameController : Node
{
    [Export]
    private MapObject mMapObject;
    public enum GameState
    {
        Prepare,
        Wave
    }

    public bool CanSetToWave()
    {
        var paths = mMapObject.FindAllPaths();
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



}
