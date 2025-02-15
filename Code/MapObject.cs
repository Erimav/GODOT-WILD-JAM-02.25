using Godot;
using System;
using System.Collections.Generic;


public partial class MapObject : Node
{
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
    private List<List<Node3D>> mBlocks = new List<List<Node3D>>();

    private Map mMap = new Map();

    public override void _Ready()
    {
    }

    private void BuildShortestPath(int startX, int startY, int endX, int endY)
    {
        List<Map.TilePosition> path = mMap.FindShortestPath(new Map.TilePosition(eStartX, eStartY), new Map.TilePosition(eEndX, eEndY));
        for(int i = 0; i < path.Count; ++i)
        {
            GD.Print("Path position " + i + ": " + path[i]);
            mBlocks[path[i].mY][path[i].mX].Scale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
    private void GenerateMap()
    {
        foreach(List<Node3D> blocks in mBlocks)
        {
            foreach(Node3D node in blocks)
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
            mBlocks.Add(new List<Node3D>());
            var row = mBlocks[mBlocks.Count - 1];
            for (int j = 0; j < eWidth; ++j)
            {
                var block = (Node3D)eMapBlock.Instantiate();
                AddChild(block);
                block.Position = eMapStartPosition + rowOffset + colOffset;
                GD.Print("Block Position: " + block.Position);
                colOffset = new Vector3(colOffset.X, colOffset.Y, colOffset.Z + eBlockColMargin);
                row.Add(block);
            }
            rowOffset = new Vector3(rowOffset.X + eBlockRowMargin, rowOffset.Y, rowOffset.Z);
        }

        mMap.AddMimics(eMimicNumber);
    }
}
