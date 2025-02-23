using Godot;
using System;
using System.Threading;

public partial class Tile : Node3D
{
	[Signal]
	public delegate void isBlockPressedEventHandler(Tile block);

    [Export]
    private PackedScene[] eBoulderMeshVariants;
    [Export]
    private PackedScene[] eClearTileMeshVariants;
	[Export]
	private PackedScene[] eMimicVariants;
	[Export]
	private PackedScene eClearEffect;

	private Node mTileContent;
	private Node mRevealedSphere;

	private bool mIsRevealed = false;
	public bool IsRevealed => mIsRevealed;

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
		Node clearEffect = eClearEffect.Instantiate();

		AddChild(clearEffect);
		if (mRevealedSphere is not null)
		{
			RemoveRevealSphere();
        }
    }

	public void AddBoulder()
	{
        PackedScene blockPrefab = eBoulderMeshVariants[new Random().Next(eBoulderMeshVariants.Length)];
        var block = blockPrefab.Instantiate();
        AddChild(block);
        mTileContent = block;
    }

	public void AddMimic()
	{
		mTileContent.QueueFree();
        PackedScene tilePrefab = eMimicVariants[new Random().Next(eMimicVariants.Length)];
        var mimicTile = tilePrefab.Instantiate();
        AddChild(mimicTile);
        mTileContent = mimicTile;
        Node3D clearEffect = eClearEffect.Instantiate<Node3D>();
		clearEffect.Scale *= 2.5f;
        AddChild(clearEffect);
    }

	public void SetTileContent(Node tileContent)
	{
		if (mTileContent is not null)
		{
			mTileContent.QueueFree();
		}
		AddChild(tileContent);
		mTileContent = tileContent; 
	}

	public void ChangeColor(float r, float g, float b)
	{
		/*MeshInstance3D mesh = GetNode<MeshInstance3D>("TileMesh");
		Variant color = new Color(r, g, b);
		Material material = mesh.GetSurfaceOverrideMaterial(0);
		material.Set("albedo_color", color);
        mesh.SetSurfaceOverrideMaterial(0, material);*/
	}

	public void RemoveRevealSphere()
	{
		if (IsInstanceValid(mRevealedSphere))
		{
			mIsRevealed = false;

            mRevealedSphere.QueueFree();
		}
    }

	public void RevealSphere(Color color, float time)
	{
		if (mIsRevealed) return;

        var revealSphere = new MeshInstance3D();
        revealSphere.Scale = new Vector3(1.5f, 1.5f, 1.5f);
        revealSphere.Mesh = new SphereMesh();
        revealSphere.MaterialOverride ??= new StandardMaterial3D();
        revealSphere.MaterialOverride.Set("transparency", 1);
        revealSphere.MaterialOverride.Set("albedo_color", color);
		AddChild(revealSphere);
		
		mRevealedSphere = revealSphere;
		mIsRevealed = true;
		
		Color finalColor = color;
		finalColor.A = 0;
		
		var tween = CreateTween().SetParallel();
		tween.TweenProperty(revealSphere.MaterialOverride, "albedo_color", finalColor, time);
		tween.TweenCallback(Callable.From(() => {
			RemoveRevealSphere();
		})).SetDelay(time);
    }

	public void OnMouseEntered()
	{
		foreach(var child in mTileContent.GetChildren(true))
		{
			if (child is MeshInstance3D meshInstance)
			{
				for (int i = 0; i < meshInstance.GetSurfaceOverrideMaterialCount(); ++i)
				{
                    ShaderMaterial shaderMaterial = null;

					Material material = meshInstance.GetSurfaceOverrideMaterial(i);

					if (material is not null)
					{
						if (material is ShaderMaterial shaderMat)
						{
							shaderMaterial = shaderMat;
						}
						else if (material.NextPass is ShaderMaterial shaderMat0)
						{
							shaderMaterial = shaderMat0;
						}
						if (shaderMaterial is not null) 
							shaderMaterial.SetShaderParameter("isHovered", true);
					}
                }
			}
		}
	}

	public void OnMouseExited()
	{
        foreach (var child in mTileContent.GetChildren())
        {
            if (child is MeshInstance3D meshInstance)
            {
                for (int i = 0; i < meshInstance.GetSurfaceOverrideMaterialCount(); ++i)
                {
                    ShaderMaterial shaderMaterial = null;

                    Material material = meshInstance.GetSurfaceOverrideMaterial(i);

                    if (material is not null)
                    {
                        if (material is ShaderMaterial shaderMat)
                        {
                            shaderMaterial = shaderMat;
                        }
                        else if (material.NextPass is ShaderMaterial shaderMat0)
                        {
                            shaderMaterial = shaderMat0;
                        }
                        if (shaderMaterial is not null)
                            shaderMaterial.SetShaderParameter("isHovered", false);
                    }
                }
            }
        }
    }
}
