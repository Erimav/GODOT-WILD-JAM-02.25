using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyController : Node
{
    [Export]
    private MapObject eMapObject;
    [Export]
    private PackedScene[] eTowersPrefabs;

    private List<(float, PackedScene)> mTowerByPrice;
    // PUBLIC METHODS

    public override void _Ready()
    {
        mTowerByPrice = new List<(float, PackedScene)> ();
        for (int i = 0; i < eTowersPrefabs.Length; ++i)
        {
            Tower tower = eTowersPrefabs[i].Instantiate<Tower>();
            mTowerByPrice.Add((tower.Price, eTowersPrefabs[i]));
            tower.QueueFree();
        }
        base._Ready();
    }

    public void OnPrepare()
    {

    }

    // PRIVATE METHODS
    private float GetResourceForTheWave(int waveNumber)
    {
        return waveNumber * 150;
    }
}
