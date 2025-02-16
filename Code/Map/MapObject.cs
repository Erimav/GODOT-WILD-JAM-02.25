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

    [ExportCategory("MapParameters")]
    [Export]
    private int eWidth;
    [Export]
    private int eHeight;
    [Export]
    private int eMimicNumber;

    [ExportCategory("MapBlockRender")]
    [Export]
    private PackedScene eMapBlock;
    [Export]
    private Vector3 eMapStartPosition;
    [Export]
    private float eBlockColMargin;
    [Export]
    private float eBlockRowMargin;
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

    // PRIVATE
    private List<List<Tile>> mBlocks = new List<List<Tile>>();

    private Map mMap = new Map();

    public override void _Ready()
    {
    }

    private void BuildShortestPath(int startX, int startY, int endX, int endY)
    {
        List<TilePosition> path = mMap.FindShortestPath(new TilePosition(eStartX, eStartY), new TilePosition(eEndX, eEndY));
        for(int i = 0; i < path.Count; ++i)
        {
            GD.Print("Path position " + i + ": " + path[i]);
            mBlocks[path[i].mY][path[i].mX].Scale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
    private void GenerateMap()
    {
        foreach(List<Tile> blocks in mBlocks)
        {
            foreach(Tile node in blocks)
            {
                node.QueueFree();
            }
        }
        mBlocks.Clear();

        mMap.GenerateMap(eWidth, eHeight);

        Vector3 rowOffset = Vector3.Zero;
        for (int i = 0; i < eHeight; ++i)
        {
            Vector3 colOffset = Vector3.Zero;
            mBlocks.Add(new List<Tile>());
            var row = mBlocks[mBlocks.Count - 1];
            for (int j = 0; j < eWidth; ++j)
            {
                var block = (Tile)eMapBlock.Instantiate();
                AddChild(block);

                block.Position = eMapStartPosition + rowOffset + colOffset;

                GD.Print("Block Position: " + block.Position);

                colOffset = new Vector3(colOffset.X, colOffset.Y, colOffset.Z + eBlockColMargin);

                block.Connect("isBlockPressed", new Godot.Callable(this,"onBlockClicked"));

                row.Add(block);
            }
            rowOffset = new Vector3(rowOffset.X + eBlockRowMargin, rowOffset.Y, rowOffset.Z);
        }

        mMap.AddMimics(eMimicNumber);
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
                    if (block == mBlocks[i][j])
                    {
                        EmitSignal("isBlockPressed", mBlocks[i][j], j, i);
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
                mBlocks[tilePosition.mY][tilePosition.mX].Destroy();
                EmitSignal("blockErased");
                return;
            }
        }
    }
}
