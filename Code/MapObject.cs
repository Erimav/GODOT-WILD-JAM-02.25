using Godot;
using System;

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
    private bool eGenerateMap
    {
        set {
            GD.Print("eGenerate is " + value);
            GenerateMap();
        }
        get { return false; }
    }

    private Map mMap = new Map();

    public override void _Ready()
    {
    }

    private void GenerateMap()
    {
        mMap.GenerateMap(eWidth, eHeight);

        Vector3 rowOffset = Vector3.Zero;
        for (int i = 0; i < eHeight; ++i)
        {
            Vector3 colOffset = Vector3.Zero;
            for (int j = 0; j < eWidth; ++j)
            {
                var block = (Node3D)eMapBlock.Instantiate();
                AddChild(block);
                block.Position = eMapStartPosition + rowOffset + colOffset;
                GD.Print("Block Position: " + block.Position);
                colOffset = new Vector3(colOffset.X, colOffset.Y, colOffset.Z + eBlockColMargin);
            }
            rowOffset = new Vector3(rowOffset.X + eBlockRowMargin, rowOffset.Y, rowOffset.Z);
        }
    }
}
