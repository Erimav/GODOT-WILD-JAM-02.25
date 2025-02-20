using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class EnemyController : Node
{
    [Export]
    private MapObject eMapObject;
    [Export]
    private GameController eGameController;
    [Export]
    private PackedScene[] eTowersPrefabs;

    private List<(Tower, PackedScene)> mTowers;

    private float mResources = 0; 
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
        mResources += GetResourceForTheWave(waveNumber);
        float maxResourcesSpentForBoulders = mResources * 0.3f;
        float resourcesSpentForBoulders = 0;
        List<TilePosition> clearedTiles = eMapObject.GetTilesByTileFill(new TileFill(false, true, false)); 
        while (mResources > 0 && resourcesSpentForBoulders < maxResourcesSpentForBoulders && clearedTiles.Count > 0)
        {
            TilePosition randomClearedPosition = clearedTiles[new Random().Next(clearedTiles.Count)];
            if (eMapObject.TryRestoreBoulder(randomClearedPosition))
            {
                resourcesSpentForBoulders += 25.0f;
            }
            clearedTiles.Remove(randomClearedPosition);
        }
        mResources -= resourcesSpentForBoulders;
        eMapObject.AddMimics(waveNumber + 3);
    }

    public void OnWaveBegin(int waveNumber)
    {
        GD.Print("Enemy Controller, On Wave begin - wave number (" + waveNumber + ")");
        GD.Print("Enemy Controller. On Wave begin - resources (" + mResources + ")");
        float cheapestTower = float.MaxValue;
        // Spawn Boulders on Random Cleared Tiles


        // Spawn Random Tower
        foreach(var tower in mTowers)
        {
            if (tower.Item1.Price < cheapestTower)
                cheapestTower = tower.Item1.Price;
        }

        while (mResources >= cheapestTower)
        {
            List<(Tower, PackedScene)> possibleTowers = new List<(Tower, PackedScene)>();
            foreach(var tower in mTowers)
            {
                if (tower.Item1.Price <= mResources)
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
            mResources -= curTower.Item1.Price;
            
        }
    }

    // PRIVATE METHODS
    private float GetResourceForTheWave(int waveNumber)
    {
        return waveNumber * 150.0f;
    }
}
