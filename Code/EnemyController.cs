using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyController : Node
{
    [Export]
    private MapObject eMapObject;
    [Export]
    private GameController eGameController;
    [Export]
    private PackedScene[] eTowersPrefabs;

    private List<(Tower, PackedScene)> mTowers;
    // PUBLIC METHODS

    public override void _Ready()
    {
        mTowers = new List<(Tower, PackedScene)> ();
        for (int i = 0; i < eTowersPrefabs.Length; ++i)
        {
            Tower tower = eTowersPrefabs[i].Instantiate<Tower>();
            mTowers.Add((tower, eTowersPrefabs[i]));
            //tower.QueueFree();
        }
        base._Ready();
    }

    public void OnPrepare(int waveNumber)
    {
        GD.Print("Enemy Controller, On Prepare - wave number (" + waveNumber + ")");
        float resources = GetResourceForTheWave(waveNumber);
        float cheapestTower = float.MaxValue;
        // Spawn Random Tower
        foreach(var tower in mTowers)
        {
            if (tower.Item1.Price < cheapestTower)
                cheapestTower = tower.Item1.Price;
        }

        while (resources > cheapestTower)
        {
            List<(Tower, PackedScene)> possibleTowers = new List<(Tower, PackedScene)>();
            foreach(var tower in mTowers)
            {
                if (tower.Item1.Price <= resources)
                    possibleTowers.Add((tower));
            }
            Random random = new Random();
            var curTower = possibleTowers[random.Next(possibleTowers.Count)];
            Dictionary<int, List<TilePosition>> tilePriorities = eMapObject.GetTileTowerPriorities(curTower.Item1);
            int biggestPriority = int.MinValue;
            foreach (int key in tilePriorities.Keys)
            {
                if (biggestPriority < key)
                {
                    biggestPriority = key;
                }
            }
            GD.Print("Enemy Controller: Tile Priorities. Biggest Priority - " + biggestPriority + ". Number of Tiles - " + tilePriorities[biggestPriority].Count);
            if (biggestPriority <= 0 || tilePriorities[biggestPriority].Count == 0) break;
            TilePosition towerPosition = tilePriorities[biggestPriority][random.Next(tilePriorities[biggestPriority].Count)];
            //tilePriorities[biggestPriority].Remove(towerPosition);
            eMapObject.ErrectTower(towerPosition, curTower.Item2);
            resources -= curTower.Item1.Price;
            
        }
    }

    // PRIVATE METHODS
    private float GetResourceForTheWave(int waveNumber)
    {
        return waveNumber * 150.0f;
    }
}
