using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map
{
	private List<List<TilePosition>> mMap;
	private List<List<TileFill>> mMapFilled;
	private int mWidth;
	private int mHeight;

    // PRIVATE

    private List<TilePosition> mOffsets = new List<TilePosition>()
        {
            new TilePosition(-1, 0),
            new TilePosition(0, -1),
            new TilePosition(1, 0),
            new TilePosition(0, 1)
        };

	private List<TilePosition> mLargeOffsets = new List<TilePosition>()
	{
		new TilePosition(-2, 0),
		new TilePosition(-1, -1),
		new TilePosition(0, -2),
		new TilePosition(1, -1),
		new TilePosition(2, 0),
		new TilePosition(1, 1),
		new TilePosition(0, 2),
		new TilePosition(-1, 1),

	};

    // PRIVATE
    private int CalculateTilePriority(TilePosition tile, List<TilePosition> offsets, Dictionary<int, List<TilePosition>> tilePriorities)
	{
		int priority = 0;
		foreach (var offset in offsets)
		{
			TilePosition neighbour = tile + offset;
			if (IsOnMap(neighbour))
			{
				if (tilePriorities[-1].Find((tile) => tile == neighbour) != null)
				{
					if (offset.GetModuleDistance() == 2) priority += 1;
					else if (offset.GetModuleDistance() == 1) priority += 2;
				}
			}
		}
		return priority;
	}

    // PUBLIC
    public bool IsOnMap(TilePosition tilePosition)
	{
		return !(tilePosition.mCol < 0 || tilePosition.mCol >= mWidth || tilePosition.mRow < 0 || tilePosition.mRow >= mHeight);
	}
	
	public List<TilePosition> GetPlayerEndPositions()
	{
        List<TilePosition> openEndPositions = new List<TilePosition>();
        for (int i = 0; i < mHeight; ++i)
        {
            openEndPositions.Add(mMap[i][mWidth - 1]);
        }
        return openEndPositions;
    }

    public List<TilePosition> GetPlayerOpenEndPositions()
    {
		List<TilePosition> endPositions = GetPlayerEndPositions();
		List<TilePosition> openEndPositions = new List<TilePosition>();
        foreach (var endPosition in endPositions)
        {
            TileFill tileFill = mMapFilled[endPosition.mRow][endPosition.mCol];
            if (tileFill.isClear)
            {
                openEndPositions.Add(mMap[endPosition.mRow][endPosition.mCol]);
            }
        }
        return openEndPositions;
    }

	public List<TilePosition> GetPlayerStartPositions()
	{
        List<TilePosition> openEndPositions = new List<TilePosition>();
        for (int i = 0; i < mHeight; ++i)
        {
            openEndPositions.Add(mMap[i][0]);
        }
        return openEndPositions;
    }

    public List<TilePosition> GetPlayerOpenStartPositions()
    {
        List<TilePosition> openStartPositions = new List<TilePosition>();
		List<TilePosition> startPositions = GetPlayerStartPositions();
        foreach (var startPosition in startPositions)
        {
            TileFill tileFill = mMapFilled[startPosition.mRow][startPosition.mCol];
            if (tileFill.isClear)
            {
                openStartPositions.Add(mMap[startPosition.mRow][startPosition.mCol]);
            }
        }
        return openStartPositions;
    }

    public void GenerateMap(int width, int height)
	{
		mWidth = width;
		mHeight = height;
		
		GD.Print("Map.GenerateMap");
		
		mMap = new List<List<TilePosition>>();
        mMapFilled = new List<List<TileFill>>();

        for (int i = 0; i <  height; ++i)
		{
			mMap.Add(new List<TilePosition>());
			mMapFilled.Add(new List<TileFill>());

			var row = mMap[mMap.Count - 1];
			var rowFilled = mMapFilled[mMap.Count - 1];

			for (int j = 0; j < width; ++j)
			{
				row.Add(new TilePosition(j, i));
                rowFilled.Add(new TileFill());

            }
		}

		GD.Print("Map Generated: " + MapToString());
	}

	public TileFill GetTileFill(int x, int y)
	{
		return GetTileFill(new TilePosition(x, y));
	}

	// Can return null if tileposition is not on the map
	public TileFill GetTileFill(TilePosition tilePosition)
	{
		if (!IsOnMap(tilePosition))
		{
			GD.Print("Get Tile Fill return default because: " + tilePosition + " is not on the map");
			return null;
		}

		return mMapFilled[tilePosition.mRow][tilePosition.mCol];
	}

	public void AddMimics(int number)
	{
		// BLESS RNGesus
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		
		List<TilePosition> startPositions = GetPlayerStartPositions();
		List<TilePosition> endPositions   = GetPlayerEndPositions();
		List<TilePosition> canSpawnMimic = new List<TilePosition>();

		foreach (var tileList in mMap)
		{
			foreach (var tile in tileList)
			{
				TileFill tileFill = mMapFilled[tile.mRow][tile.mCol];
				Predicate<TilePosition> predicate = (TilePosition tilePosition) => { return tile == tilePosition; };
				if (startPositions.Find(predicate)  != null ||
					endPositions.Find(predicate)	!= null ||
                    !tileFill.CanSpawnMimic())
				{
					continue;
				}
				else
				{
					canSpawnMimic.Add(tile);
				}
			}
		}

		for (int i = 0; i < number; ++i)
		{
			if (canSpawnMimic is null && canSpawnMimic.Count == 0) break;
			TilePosition mimicPosition = canSpawnMimic[RNG.RandiRange(0, canSpawnMimic.Count)];

			GD.Print("Tile fetched by Random coordinate: " + mimicPosition);
			mMapFilled[mimicPosition.mRow][mimicPosition.mCol].isMimic = true;
			canSpawnMimic.Remove(mimicPosition);
        }
	}

	public List<List<TilePosition>> FindPaths(TilePosition startPosition, List<TilePosition> endPosition)
	{
		return new List<List<TilePosition>>();
	}

	public List<TilePosition> GetShortestPath()
	{
        var paths = FindAllShortestPaths();
        //GD.Print("MapObject: Num of Paths - " + paths.Count);

        if (paths.Count == 0) return new List<TilePosition>();
        else
        {
            List<TilePosition> shortestPath = new List<TilePosition>();
            int shortestPathLength = int.MaxValue;
            foreach (List<TilePosition> path in paths.Values)
            {
                if (path.Count < shortestPathLength)
                {
                    shortestPath = path;
                    shortestPathLength = path.Count;
                }
            }
            return shortestPath;
        }
    }

	public Dictionary<TilePosition, List<TilePosition>> FindAllShortestPaths()
	{
        List<TilePosition> startPositions = GetPlayerOpenStartPositions();
        List<TilePosition> endPositions = GetPlayerOpenEndPositions();

        Dictionary<TilePosition, List<TilePosition>> paths = new Dictionary<TilePosition, List<TilePosition>>();

        foreach (TilePosition startPosition in startPositions)
        {
            foreach (TilePosition endPosition in endPositions)
            {
                List<TilePosition> shortestPath = new List<TilePosition>();
                int pathLength = int.MaxValue;

                List<TilePosition> path = FindShortestPath(startPosition, endPosition);

                if (path.Count < pathLength)
                {
                    shortestPath = path;
                    pathLength = path.Count;
                }

                if (shortestPath.Count > 0)
                {
                    if (paths.ContainsKey(startPosition))
                        paths[startPosition] = shortestPath;
                    else
                        paths.Add(startPosition, shortestPath);
                }
            }
        }
        return paths;
    }

	// THIS IS ASS SCARY FUNCTION AND I"M TOO SLEEPY TO UNDERSTAND WHAT HAPPENS HERE
	public List<TilePosition> FindShortestPath(TilePosition startPosition, TilePosition endPosition)
	{
		if (!IsOnMap(startPosition) || !IsOnMap(endPosition))
		{
			GD.Print("Find Shortes Path: TilePosition is not on map");
			return new List<TilePosition>();
		}

		List<TilePosition> path = new List<TilePosition>();

		bool[,] tileStates = new bool[mHeight, mWidth];
		TilePosition[,] tilePrevious = new TilePosition[mHeight, mWidth];

		int[,] tileEstimation = new int[mHeight, mWidth];
        
		for (int i = 0; i < tileEstimation.Length; i++)
        {
			int row = i / mWidth;
			int col = i % mWidth;
			tileEstimation[row, col] = -1;
        };

		Stack<TilePosition> tilePositions = new Stack<TilePosition>();
		tilePositions.Push(startPosition);
		tileEstimation[startPosition.mRow, startPosition.mCol] = 0;

		bool pathFound = false;

		while (tilePositions.Count > 0)
		{
			TilePosition currentPosition = tilePositions.Pop();
			tileStates[currentPosition.mRow, currentPosition.mCol] = true;

			List<TilePosition> neighbours = new List<TilePosition>();

			foreach (TilePosition offset in mOffsets)
			{
				TilePosition neighbour = currentPosition + offset;
				if (neighbour.mCol < 0 || neighbour.mCol >= mWidth ||
					neighbour.mRow < 0 || neighbour.mRow >= mHeight)
				{
					continue;
				}
				if (tileStates[neighbour.mRow, neighbour.mCol])
				{
					continue;
				}
				if (!mMapFilled[neighbour.mRow][neighbour.mCol].isClear)
				{
					continue;
				}
				
				int newEstimate = tileEstimation[currentPosition.mRow, currentPosition.mCol] + 1;
				if (tileEstimation[neighbour.mRow,neighbour.mCol] == -1 || tileEstimation[neighbour.mRow, neighbour.mCol] > newEstimate)
				{
					tileEstimation[neighbour.mRow, neighbour.mCol] = newEstimate;
					tilePrevious[neighbour.mRow, neighbour.mCol] = currentPosition;
				}
				neighbours.Add(neighbour);
			}

			neighbours.Sort((TilePosition x, TilePosition y) => 
			{
				int xEstimation = tileEstimation[x.mRow, x.mCol];
				int yEstimation = tileEstimation[y.mRow, y.mCol];
				if (xEstimation < yEstimation) return 1;
				if (xEstimation == yEstimation) return 0;
				if (xEstimation > yEstimation) return -1;
				return 0; 
			});

			foreach (TilePosition neighbour in neighbours)
			{
				tilePositions.Push(neighbour);
			}

			if (tilePrevious[endPosition.mRow, endPosition.mCol] != null)
			{
				pathFound = true;
                break;
			}
		}

		if (pathFound)
		{
			TilePosition pathPoint = endPosition;
			while(pathPoint != startPosition)
			{
				path.Add(pathPoint);
				pathPoint = tilePrevious[pathPoint.mRow, pathPoint.mCol];
			}
			path.Add(startPosition);
			path.Reverse();
		}

		return path;
	}

	public Dictionary<int, List<TilePosition>> GetTileTowerPriorities(Tower tower)
	{
        Dictionary<int, List<TilePosition>> tilePriorities = new Dictionary<int, List<TilePosition>>();
		List<TilePosition> path = GetShortestPath();
        foreach (var tile in path)
        {
            int x = tile.mCol;
            int y = tile.mRow;
            if (path.Find((tile) => {
				return tile.mCol == x && tile.mRow == y;
				}) != null)
			{
				// -1 means it's the shortest path
				if (!tilePriorities.ContainsKey(-1))
				{
					tilePriorities.Add(-1, new List<TilePosition> { tile });
				}
				else
				{
					tilePriorities[-1].Add(tile);

				}
			}
		}

		List<TilePosition> offsets = mLargeOffsets;
		offsets.AddRange(mOffsets);
		foreach (var tileList in mMap)
		{
			foreach (var tile in tileList)
			{
				int x = tile.mCol;
				int y = tile.mRow;
				TileFill tileFill = mMapFilled[y][x];
				if (tileFill.isMimic || tileFill.isTower || 
					(tilePriorities[-1].Find((TilePosition t) => t == tile) != null))
				{
					//GD.Print("GetTileTowerPriorities - Tile Occupied and skipped");
					continue;
				}
				int priority = CalculateTilePriority(tile, offsets, tilePriorities);
				if (!tilePriorities.ContainsKey(priority))
				{
					tilePriorities[priority] = new List<TilePosition>() { tile };
				}
				else
				{
					tilePriorities[priority].Add(tile);

				}
			}
			
		}
		return tilePriorities;
	}

    // PUBLIC DEBUG

    public string MapToString()
	{
		string str = "";
		foreach (List<TilePosition> list in mMap)
		{
			str += "\n";
			foreach (TilePosition pos in list)
			{
				str += pos.ToString() ;
			}
		}
		return str;
	}
}
