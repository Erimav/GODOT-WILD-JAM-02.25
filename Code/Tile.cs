using Godot;
using System;

public partial class Tile : Node3D
{
	[Signal]
	public delegate void isBlockPressedEventHandler(Tile block);

	// Private
	private MeshInstance3D mMesh;

    public override void _Ready()
    {
		mMesh = GetNode<MeshInstance3D>("MeshInstance3D");
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

	public void Destroy()
	{
		mMesh.Mesh = new PlaneMesh();
	}
}
