using Godot;
using System;
using System.Collections.Generic;

public partial class Map
{
	public class TilePosition
	{
		public int mX;
		public int mY;
		public TilePosition(int x, int y)
		{
			mX = x;
			mY = y;
		}

		public static TilePosition operator+(TilePosition left, TilePosition right)
		{
			return new TilePosition(left.mX + right.mX, left.mY + right.mY);
		}

        public static TilePosition operator -(TilePosition left, TilePosition right)
        {
            return new TilePosition(left.mX - right.mX, left.mY - right.mY);
        }

		public static bool operator==(TilePosition left, TilePosition right)
		{
			return (left.mX == right.mX && left.mY == right.mY);
		}

        public static bool operator!=(TilePosition left, TilePosition right)
        {
            return (left.mX != right.mX || left.mY != right.mY);
        }

        public override string ToString()
        {
            return "(x: " + mX + " | " + "y: " + mY + ")";
        }
    }

	private List<List<TilePosition>> mMap;
	private List<TilePosition> mMimics;
	private int mWidth;
	private int mHeight;

	// PRIVATE

	// PUBLIC
	public void GenerateMap(int width, int height)
	{
		GD.Print("Map.GenerateMap");
		mMap = new List<List<TilePosition>>();
		for (int i = 0; i <  height; ++i)
		{
			mMap.Add(new List<TilePosition>());
			var row = mMap[mMap.Count - 1];
			for (int j = 0; j < width; ++j)
			{
				row.Add(new TilePosition(j, i));
			}
		}

		GD.Print("Map Generated: " + MapToString());
	}

	public void TryEraseTile(int x, int y)
	{

	}

	public void TryEraseTile(TilePosition tilePosition)
	{

	}

	public void AddMimics(int number)
	{

	}

	public List<List<TilePosition>> FindPaths(TilePosition startPosition, TilePosition endPosition)
	{
		return new List<List<TilePosition>>();
	}

	public List<TilePosition> FindShortestPath(TilePosition startPosition, TilePosition endPosition)
	{
		return new List<TilePosition>();
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
