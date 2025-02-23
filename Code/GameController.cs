using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node
{
    [Signal]
    public delegate void GameStateSetToWaveEventHandler(int waveNumber);

    [Signal]
    public delegate void GameStateSetToPrepareEventHandler(int waveNumber);

    [Signal]
    public delegate void GameWonEventHandler();

    [Signal]
    public delegate void GameLostEventHandler();

    [Signal]
    public delegate void GameStartedEventHandler();


    [ExportCategory("External Exports")]
    [Export]
    private MobController eMobController;
    [Export]
    private PlayerController ePlayerController;
    [Export]
    private EnemyController eEnemyController;

    [Export]
    private MapObject eMapObject;

    [ExportCategory("Game Parameters")]
    [Export]
    private int eSpawnNumber;
    [Export]
    private int eMaxWave;
    [Export]
    private int eMobFinishedToWin;
    [Export]
    private int eStartBalance;


    // PRIVATE
    private int mWaveNumber = 0;
    private bool mIsGameOnPause = false;
    // PUBLIC

    public int SpawnNumber
    {
        get => eSpawnNumber;
    }
    public int MaxWave
    {
        get => eMaxWave;
    }
    public int WaveNumber
    {
        get => mWaveNumber;
    }
    public int MobFinishedPathToWin
    {
        get => eMobFinishedToWin;
    }
    public int MobFinishedPath
    {
        get => eMobController.MobFinishedPath;
    }

    // PUBLIC METHODS

    public override void _Ready()
    {
        base._Ready();
        StartGame();
        SetPrepare();
    }

    public void ResetGame()
    {
        mWaveNumber = 0;
        ePlayerController.Reset();
        eMobController.Reset();
        eEnemyController.Reset();
        eMapObject.GenerateMap();
    }

    public void PauseGame()
    {
        ProcessMode = Node.ProcessModeEnum.Pausable;
        mIsGameOnPause = true;
    }

    public void UnPauseGame()
    {
        ProcessMode = Node.ProcessModeEnum.Always;
        mIsGameOnPause = false;
    }

    public void StartGame()
    {
        ResetGame();
        UnPauseGame();
        Wallet.Balance = eStartBalance;
        EmitSignal(SignalName.GameStarted);
    }

    public override void _Process(double delta)
    {
        if (!mIsGameOnPause)
        {
            List<TilePosition> path = eMapObject.TakeShortestPath();
            if (mWaveNumber == 10 || (path.Count == 0 && Wallet.Balance < eMapObject.ClearTilePrice))
            {
                GD.Print("Game Lost");
                EmitSignal(SignalName.GameLost);
                PauseGame(); 
            }

            if (eMobController.MobFinishedPath == eMobFinishedToWin)
            {
                GD.Print("Game Won");
                EmitSignal(SignalName.GameWon);
                PauseGame();
            }
        }
        base._Process(delta);
    }

    public bool CanSetToWave()
    {
        if (GameManager.GetInstance().GetGameState() != GameManager.GameState.Prepare ) 
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
            EmitSignal("GameStateSetToWave", mWaveNumber);
            return true;
        }
        return false;
    }

    public void SetPrepare()
    {
        GameManager.GetInstance().ChangeState(GameManager.GameState.Prepare);
        EmitSignal("GameStateSetToPrepare", ++mWaveNumber);
    }

    public void StartWave()
    {
        if (TryToToSetWave())
        {
            GD.Print("Game Controller: Start Wave");
            eMobController.StartSpawn(eSpawnNumber + mWaveNumber * 4);
            eMobController.Connect("NoMoreMobsOnWave", new Callable(this, "EndWave"), (uint)ConnectFlags.OneShot);
        }
    }

    public void EndWave()
    {
        SetPrepare();
    }
}
