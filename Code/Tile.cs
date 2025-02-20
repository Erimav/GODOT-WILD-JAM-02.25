using Godot;
using System;

public partial class Tile : Node3D
{
	[Signal]
	public delegate void isBlockPressedEventHandler(Tile block);

    [Export]
    private PackedScene[] eBoulderMeshVariants;
    [Export]
    private PackedScene[] eClearTileMeshVariants;
	[Export]
	private PackedScene eClearEffect;

	private Node mTileContent;

    public override void _Ready()
    {
		AddBoulder();
    }

    void OnInputEvent(Node camera, InputEvent inputEvent, Vector3 eventPosition, Vector3 normal, int shape_idx)
	{
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				GD.Print("Tile: On Input Event. Left Click Pressed");
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

	public void AddBoulder()
	{
        PackedScene blockPrefab = eBoulderMeshVariants[new Random().Next(eBoulderMeshVariants.Length)];
        var block = blockPrefab.Instantiate();
        AddChild(block);
        mTileContent = block;
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
