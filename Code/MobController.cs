using Godot;
using System;

public partial class MobController : Node
{
	[ExportCategory("External Exports")]
	[Export]
	private Timer eTimer;
	[Export]
	private MapObject eMapObject;

	[ExportCategory("Mobs scenes")]
	[Export]
	private PackedScene eGoblin;

	// PRIVATE
	private int mTimerCount = 0;
	private int mTimerEndCount = 0;

	public void StartSpawn(int mobNumber)
	{
		GD.Print("Mob Controller: Start Spawn " + mobNumber);
		mTimerEndCount = mobNumber;
		mTimerCount = 0;
		eTimer.Timeout += SpawnMob;
		eTimer.Timeout += EndTimer;
		eTimer.Start();
	}

	private void SpawnMob()
	{
		GD.Print("Mob Controller: Spawn Mob");
		Mob mob = eGoblin.Instantiate<Mob>();
		eMapObject.AddMobToMap(mob);
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

}
