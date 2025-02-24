using Godot;
using System;

public partial class MobController : Node
{
	[Signal]
	public delegate void NoMoreMobsOnWaveEventHandler();
    [Signal]
    public delegate void MobSpawnedEventHandler(Mob mob);

	[ExportCategory("External Exports")]
	[Export]
	private Timer eTimer;
	[Export]
	private MapObject eMapObject;
    [Export]
    private EnemyController eEnemyController;
    [Export]
    private PlayerController ePlayerController;

	[ExportCategory("Mobs scenes")]
	[Export]
	private PackedScene eGoblin;

	// PRIVATE
	private int mTimerCount = 0;
	private int mTimerEndCount = 0;
    private int mMobNumber = -1;
    private int mMobFinishedPath = 0;

    // PUBLIC
    public int MobFinishedPath
    {
        get => mMobFinishedPath;
    }

	// PRIVATE METHOD
    private void SpawnMob()
    {
        GD.Print("Mob Controller: Spawn Mob");
        Mob mob = eGoblin.Instantiate<Mob>();

        mob.OnMobDeadPath += (Mob mob) => { mMobNumber--; };
        mob.OnMobDeadPath += (Mob mob) => { eEnemyController.AddToResource(mob.ResourceValue / 5); };
        mob.OnMobFinishedPath += (Mob mob) => { mMobNumber--; };
        mob.OnMobFinishedPath += (Mob mob) => { mMobFinishedPath++; };

        mob.OnMobFinishedPath += (Mob mob) => { Wallet.Balance += mob.ResourceValue; };

        eMapObject.AddMobToMap(mob);
        EmitSignal(SignalName.MobSpawned, mob);
        mTimerCount++;
    }

    private void EndTimer()
    {
        if (mTimerCount >= mTimerEndCount)
        {
            GD.Print("Mob Controller: End Timer");
            eTimer.Timeout -= EndTimer;
            eTimer.Timeout -= SpawnMob;
            eTimer.Stop();
        }
    }

    // PUBLIC METHODS

    public void Reset()
    {
        mTimerCount = 0;
        mTimerEndCount = 0;
        mMobNumber = -1;
        mMobFinishedPath = 0;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (mMobNumber == 0)
        {
            mMobNumber--;
            EmitSignal("NoMoreMobsOnWave");
        }
        base._Process(delta);
    }

    public void StartSpawn(int mobNumber)
	{
		GD.Print("Mob Controller: Start Spawn " + mobNumber);
        mMobNumber = mobNumber;
		mTimerEndCount = mobNumber;
		mTimerCount = 0;
		eTimer.Timeout += SpawnMob;
		eTimer.Timeout += EndTimer;
		eTimer.Start();
	}

	

}
