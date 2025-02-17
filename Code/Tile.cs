using Godot;
using System;

public partial class Tile : Node3D
{
	[Signal]
	public delegate void isBlockPressedEventHandler(Tile block);

    [Export]
    private PackedScene[] eBlockMeshVariants;
    [Export]
    private PackedScene[] eClearTileMeshVariants;
	[Export]
	private PackedScene eClearEffect;

	private Node mTileContent;

    public override void _Ready()
    {
		PackedScene blockPrefab = eBlockMeshVariants[new Random().Next(eBlockMeshVariants.Length)];
		var block = blockPrefab.Instantiate();
		AddChild(block);
		mTileContent = block;
    }

    void OnInputEvent(Node camera, InputEvent inputEvent, Vector3 eventPosition, Vector3 normal, int shape_idx)
	{
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				EmitSignal("isBlockPressed", this);
			}
		}
	}

	public void ClearTile()
	{
		mTileContent.QueueFree();
		PackedScene tilePrefab = eClearTileMeshVariants [new Random().Next(eClearTileMeshVariants.Length)];
        var clearTile = tilePrefab.Instantiate();
        AddChild(clearTile);
		mTileContent = clearTile;
		Node clrearEffect = eClearEffect.Instantiate();

		AddChild(clrearEffect);
    }

	public void ChangeColor(float r, float g, float b)
	{
		/*MeshInstance3D mesh = GetNode<MeshInstance3D>("TileMesh");
		Variant color = new Color(r, g, b);
		Material material = mesh.GetSurfaceOverrideMaterial(0);
		material.Set("albedo_color", color);
        mesh.SetSurfaceOverrideMaterial(0, material);*/
	}
}
