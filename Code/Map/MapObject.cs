using Godot;
using System;
using System.Collections.Generic;


public partial class MapObject : Node
{
    // SIGNALS

    [Signal]
    public delegate void isBlockPressedEventHandler(Tile block, int colBlock, int rowBlock);

    [Signal]
    public delegate void blockErasedEventHandler();
    [Signal]
    public delegate void blockIsMimicEventHandler();
    [Signal]
    public delegate void blockIsTowerEventHandler();

    [Signal]
    public delegate void TileRevealedEventHandler(Tile tile, int colTile, int rowTile);
    [Signal]
    public delegate void TileBlownUpEventHandler(Tile tile, int colTile, int rowTile);

    //EXPORTS
    [ExportCategory("External Exports")]
    [Export]
    private Path3D ePath3D;
    [Export]
    private Node3D eStartMobPoint;
    [Export]
    private Node3D eEndMobPoint;
    [Export]
    private AudioStream eMimicFoundSFX;

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
    [Export]
    private int eClearTilePrice;
    public int ClearTilePrice => eClearTilePrice;

    //[ExportCategory("ShopItemsParameters")]
    //[Export]
    //private PackedScene eMimicSign;

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
        set
        {
            GD.Print("eGenerate is " + value);
            //GenerateMap();
        }
        get { return false; }
    }
    [Export]
    private bool eMarkStartAndEnd
    {
        set
        {
            GD.Print("eMartStartAndEnd is " + value);
            /* for (int i = 0; i < eHeight; ++i)
             {
                 mTiles[i][0].ChangeColor(0.0f, 1.0f, 0.0f);
                 mTiles[i][eWidth - 1].ChangeColor(0.0f, 0.0f, 1.0f);
             }*/
        }
        get { return false; }
    }

    // PRIVATE
    private Tile[,] mTiles;

    private Dictionary<TilePosition, List<TilePosition>> mPaths = new Dictionary<TilePosition, List<TilePosition>>();

    private Map mMap = new Map();


    // PRIVATE METHODS

    private void BuildShortestPath(int startX, int startY, int endX, int endY)
    {
        List<TilePosition> path = mMap.FindShortestPath(new TilePosition(eStartX, eStartY), new TilePosition(eEndX, eEndY));
        for (int i = 0; i < path.Count; ++i)
        {
            GD.Print("Path position " + i + ": " + path[i]);
            mTiles[path[i].mRow, path[i].mCol].Scale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
    public void GenerateMap()
    {
        if (mTiles != null)
        {
            foreach (var tile in mTiles)
            {
                tile.QueueFree();
            }
        }

        mTiles = new Tile[eHeight, eWidth];

        mMap.GenerateMap(eWidth, eHeight);

        Vector3 rowOffset = Vector3.Zero;
        for (int i = 0; i < eHeight; ++i)
        {
            Vector3 colOffset = Vector3.Zero;
            for (int j = 0; j < eWidth; ++j)
            {
                var tile = eMapTile.Instantiate<Tile>();

                tile.Position = eMapStartPosition + rowOffset + colOffset;
                tile.Scale = eBlockScale;

                GD.Print("Block Position: " + tile.Position);

                colOffset = new Vector3(colOffset.X, colOffset.Y, colOffset.Z + eTileColMargin);

                tile.Connect("isBlockPressed", new Godot.Callable(this, "onBlockClicked"));

                mTiles[i,j] = tile;
                AddChild(tile);
            }
            rowOffset = new Vector3(rowOffset.X + eTileRowMargin, rowOffset.Y, rowOffset.Z);
        }

        AddMimics(eMimicNumber);
    }

    // PUBLIC METHODS
    public override void _Ready()
    {
        GenerateMap();
    }

    public override void _Process(double delta)
    {
        List<TilePosition> path = TakeShortestPath();
        SetupPath(path);
        base._Process(delta);

    }

    public void AddMimics(int mimicNumber)
    {
        mMap.AddMimics(mimicNumber);
    }

    public void ErrectTower(TilePosition towerPosition, PackedScene towerPrefab)
    {
        Tower tower = towerPrefab.Instantiate<Tower>();

        TileFill tileFill = mMap.GetTileFill(towerPosition);
        mTiles[towerPosition.mRow, towerPosition.mCol].ClearTile();
        tileFill.isTower = true;
        //tower.Position = new Vector3(towerPosition.mRow * eTileRowMargin, 0, towerPosition.mCol * eTileColMargin) + eMapStartPosition;
        mTiles[towerPosition.mRow, towerPosition.mCol].SetTileContent(tower);
        
    }

    public List<TilePosition> GetTilesByTileFill(TileFill predicate)
    {
        List<TilePosition> needTiles = new List<TilePosition>();
        for (int y = 0; y < eHeight; ++y)
        {
            for (int x = 0; x < eWidth; ++x)
            {
                TileFill tileFill = mMap.GetTileFill(x, y);
                if (predicate == tileFill)
                {
                    needTiles.Add(new TilePosition(x, y));
                }
            }
        }
        return needTiles;
    }

    public void onBlockClicked(Tile block)
    {
        GD.Print("onBlockClicked");
        if (mMap != null)
        {
            for (int i = 0; i < eHeight; ++i)
            {
                for (int j = 0; j < eWidth; ++j)
                {
                    if (block == mTiles[i,j])
                    {
                        EmitSignal("isBlockPressed", mTiles[i, j], j, i);
                    }
                }
            }
        }
    }

    public bool TryRestoreBoulder(int x, int y)
    {
        return TryRestoreBoulder(new TilePosition(x, y));
    }

    public bool TryRestoreBoulder(TilePosition tilePosition)
    {
        if (mMap.GetTileFill(tilePosition).isClear && !mMap.GetTileFill(tilePosition).isTower && !mMap.GetTileFill(tilePosition).isMimic)
        {

            mTiles[tilePosition.mRow, tilePosition.mCol].AddBoulder();
            mMap.GetTileFill(tilePosition).isClear = false;
            return true;
        }
        return false;
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
                mTiles[tilePosition.mRow, tilePosition.mCol].AddMimic();
                tileFill.isMimic = false;
                tileFill.isTower = true;
                AudioManager.Instance.PlaySFX(eMimicFoundSFX);
                return;
            }
            else if (tileFill.isTower)
            {
                EmitSignal("blockIsTower");
                return;
            }
            else if (!tileFill.isClear)
            {
                ForceClearTile(tilePosition, tileFill);
                Wallet.Balance -= eClearTilePrice;
                return;
            }
        }
    }

    private void ForceClearTile(TilePosition tilePosition, TileFill tileFill)
    {
        if (Wallet.Balance >= eClearTilePrice)
        {
            tileFill.isClear = true;
            mTiles[tilePosition.mRow, tilePosition.mCol].ClearTile();
            EmitSignal("blockErased");
        }
    }

    public void OnUseItem(int colTile, int rowTile)
    {
        var itemUsage = GameManager.GetInstance().ItemAtHand.Usage as FieldItemUsage;
        if (itemUsage is null)
        {
            GD.PrintErr("Trying to use non-map item");
            return;
        }

        GD.Print($"Using item to {itemUsage.UseAction} tile {colTile}:{rowTile}");
        var action = itemUsage.UseAction;

        switch (action)
        {
            case FieldItemUsage.Action.Reveal:
                RevealTile(colTile, rowTile);
                break;
            case FieldItemUsage.Action.BlowUp:
                BlowUpTile(colTile, rowTile);
                break;
        }
    }

    private void BlowUpTile(int colTile, int rowTile)
    {
        TilePosition tilePosition = new(colTile, rowTile);
        var tileFill = mMap.GetTileFill(tilePosition);
        if (tileFill.isClear)
        {
            return;
        }

        ForceClearTile(tilePosition, tileFill);
        tileFill.isTower = false;
        tileFill.isMimic = false;
        EmitSignal(SignalName.TileBlownUp, mTiles[rowTile, colTile], colTile, rowTile);
    }

    private void SpawnRevealSphere(int rowTile, int colTile, Color color, float time )
    {
        var revealSphere = new MeshInstance3D();
        revealSphere.Scale = new Vector3(1.5f, 1.5f, 1.5f);
        revealSphere.Mesh = new SphereMesh();
        revealSphere.MaterialOverride ??= new StandardMaterial3D();
        revealSphere.MaterialOverride.Set("transparency", 1);
        revealSphere.MaterialOverride.Set("albedo_color", color);
        mTiles[rowTile, colTile].AddChild(revealSphere);
    }

    private void RevealTile(int colTile, int rowTile)
    {
        
        GD.Print($"Tile {colTile}:{rowTile} try to 3x3 reveale");
        //var revealSphere = eRevealSphere.Instantiate<MeshInstance3D>();

        for (int row = -1; row < 2; ++row)
        {
            for (int col = -1; col < 2; ++col)
            {
                TilePosition tilePosition = new(col+colTile, row + rowTile);
                if (!mMap.IsOnMap(tilePosition))
                {
                    continue;
                }
                var tileFill = mMap.GetTileFill(tilePosition);
                if (tileFill.isClear)
                {
                    continue;
                }

                GD.Print($"Tile {colTile}:{rowTile} try to reveal");

                if (tileFill.isMimic)
                {
                    mTiles[tilePosition.mRow, tilePosition.mCol].RevealSphere(new Color(1.0f, 0.0f, 0.0f, 0.3f), 60.0f);
                    /*
                    var sign = new Label3D { Text = "It's Mimic!" };
                    mTiles[rowTile, colTile].AddChild(sign);
                    sign.Position = Vector3.Up * 3;
                    sign.RotationDegrees = new(90, 0, 0);*/
                }
                else if (!tileFill.isMimic && !tileFill.isTower && !tileFill.isClear)
                {
                    mTiles[tilePosition.mRow, tilePosition.mCol].RevealSphere(new Color(0.0f, 1.0f, 0.0f, 0.3f), 5.0f);
                }

                EmitSignal(SignalName.TileRevealed, mTiles[tilePosition.mRow, tilePosition.mCol], tilePosition.mCol, tilePosition.mRow);
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
        Vector3 distanceBetweenPositions = Vector3.Zero;
        if (path.Count > 1)
        {
            Tile tile0 = mTiles[path[0].mRow, path[0].mCol];
            Tile tile1 = mTiles[path[1].mRow, path[1].mCol];
            distanceBetweenPositions = tile0.GlobalPosition - tile1.GlobalPosition;
        }
        ePath3D.Curve.AddPoint(eStartMobPoint.GlobalPosition);

        for (int i = 0; i < path.Count; ++i)
        {
            TilePosition position = path[i];
            Tile tile = mTiles[position.mRow, position.mCol];
            if (i == 0)
            {
                ePath3D.Curve.AddPoint(tile.GlobalPosition + distanceBetweenPositions);
            }
            ePath3D.Curve.AddPoint(tile.Position);
            if (i == path.Count-1)
            {
                ePath3D.Curve.AddPoint(tile.GlobalPosition - distanceBetweenPositions);
            }
        }
        ePath3D.Curve.AddPoint(eEndMobPoint.GlobalPosition);
    }
    public Dictionary<int, List<TilePosition>> GetTileTowerPriorities(Tower tower)
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
