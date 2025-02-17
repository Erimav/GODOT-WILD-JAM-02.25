using Godot;
using System;
using System.Collections.Generic;


public partial class MapObject : Node
{
    // SIGNALS

    [Signal]
    public delegate void isBlockPressedEventHandler(Tile block, int xBlock, int yBlock);

    [Signal]
    public delegate void blockErasedEventHandler();
    [Signal]
    public delegate void blockIsMimicEventHandler();
    [Signal]
    public delegate void blockIsTowerEventHandler();

    //EXPORTS
    [ExportCategory("External Exports")]
    [Export]
    private Path3D ePath3D;

    [ExportCategory("MapParameters")]
    [Export]
    private int eWidth;
    [Export]
    private int eHeight;
    [Export]
    private int eMimicNumber;

    [ExportCategory("MapBlockRender")]
    [Export]
    private PackedScene eMapTile;
    [Export]
    private Vector3 eMapStartPosition;
    [Export]
    private float eTileColMargin;
    [Export]
    private float eTileRowMargin;
    [Export]
    private Vector3 eBlockScale;

    [ExportCategory("Debug")]
    [Export]
    private int eStartX;
    [Export]
    private int eStartY;
    [Export]
    private int eEndX;
    [Export]
    private int eEndY;
    [Export]
    private bool eGeneratePath
    {
        set
        {
            GD.Print("eGeneratePath is " + value);
            BuildShortestPath(eStartX, eStartY, eEndX, eEndY);
            }
        get { return false; }
    }

    [Export]
    private bool eGenerateMap
    {
        set {
            GD.Print("eGenerate is " + value);
            GenerateMap();
        }
        get { return false; }
    }
    [Export]
    private bool eMarkStartAndEnd
    { 
        set
        {
            GD.Print("eMartStartAndEnd is " + value);
            for (int i = 0; i < eHeight; ++i)
            {
                mTiles[i][0].ChangeColor(0.0f, 1.0f, 0.0f);
                mTiles[i][eWidth - 1].ChangeColor(0.0f, 0.0f, 1.0f);
            }
        }
        get { return false; }
    }

    // PRIVATE
    private List<List<Tile>> mTiles = new List<List<Tile>>();

    private Dictionary<TilePosition, List<TilePosition>> mPaths = new Dictionary<TilePosition, List<TilePosition>>();

    private Map mMap = new Map();


    // PRIVATE METHODS

    private void BuildShortestPath(int startX, int startY, int endX, int endY)
    {
        List<TilePosition> path = mMap.FindShortestPath(new TilePosition(eStartX, eStartY), new TilePosition(eEndX, eEndY));
        for (int i = 0; i < path.Count; ++i)
        {
            GD.Print("Path position " + i + ": " + path[i]);
            mTiles[path[i].mY][path[i].mX].Scale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
    private void GenerateMap()
    {
        foreach (List<Tile> blocks in mTiles)
        {
            foreach (Tile node in blocks)
            {
                node.QueueFree();
            }
        }
        mTiles.Clear();

        mMap.GenerateMap(eWidth, eHeight);

        Vector3 rowOffset = Vector3.Zero;
        for (int i = 0; i < eHeight; ++i)
        {
            Vector3 colOffset = Vector3.Zero;
            mTiles.Add(new List<Tile>());
            var row = mTiles[mTiles.Count - 1];
            for (int j = 0; j < eWidth; ++j)
            {
                var tile = eMapTile.Instantiate<Tile>();

                tile.Position = eMapStartPosition + rowOffset + colOffset;

                GD.Print("Block Position: " + tile.Position);

                colOffset = new Vector3(colOffset.X, colOffset.Y, colOffset.Z + eTileColMargin);

                tile.Connect("isBlockPressed", new Godot.Callable(this, "onBlockClicked"));

                row.Add(tile);
                AddChild(tile);
            }
            rowOffset = new Vector3(rowOffset.X + eTileRowMargin, rowOffset.Y, rowOffset.Z);
        }

        mMap.AddMimics(eMimicNumber);

    }

    // PUBLIC METHODS
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        List<TilePosition> path = TakeShortestPath();
        SetupPath(path);
        base._Process(delta);

    }

    public void onBlockClicked(Tile block)
    {
        GD.Print("onBlockClicked");
        if (mMap != null)
        {
            for(int i = 0; i < eHeight; ++i)
            {
                for(int j = 0; j < eWidth; ++j)
                {
                    if (block == mTiles[i][j])
                    {
                        EmitSignal("isBlockPressed", mTiles[i][j], j, i);
                    }
                }
            }
        }
    }

    public void OnTryEraseTile(int xTile, int yTile)
    {
        TilePosition tilePosition = new TilePosition(xTile, yTile);
        TileFill tileFill = mMap.GetTileFill(tilePosition);
        if (tileFill != null)
        {
            if (tileFill.isMimic)
            {
                EmitSignal("blockIsMimic");
                return;
            }
            if (tileFill.isTower)
            {
                EmitSignal("blockIsTower");
                return;
            }
            if (!tileFill.isClear)
            {
                tileFill.isClear = true;
                mTiles[tilePosition.mY][tilePosition.mX].ClearTile();
                EmitSignal("blockErased");
                return;
            }
        }
    }

    public Dictionary<TilePosition, List<TilePosition>> FindAllPaths()
    {
        return mMap.FindAllShortestPaths();
    }

    public void SetupPath(List<TilePosition> path)
    {
        ePath3D.Curve.ClearPoints();
        foreach (TilePosition position in path)
        {
            Tile tile = mTiles[position.mY][position.mX];
            ePath3D.Curve.AddPoint(tile.Position);
        }
    }
    public int[,] GetTileTowerPriorities(Tower tower)
    {
        return mMap.GetTileTowerPriorities(tower);
    }
    public List<TilePosition> TakeShortestPath()
    {
        var paths = FindAllPaths();
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

    public void AddMobToMap(Mob mob)
    {
        PathFollow3D pathFollow3D = new PathFollow3D();
        pathFollow3D.Loop = false;
        ePath3D.AddChild(pathFollow3D);
        mob.SetPath(pathFollow3D);
    }
}
