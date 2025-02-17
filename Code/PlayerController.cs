using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class PlayerController : Node
{
    // SIGNALS
    [Signal]
    public delegate void TryEraseTileEventHandler(int xTile, int yTile);
    public void TilePressed(Tile tile, int xTile, int yTile)
    {
        GD.Print("PlayerController: Tile Pressed. Try to Erase");
        if (GameManager.GetInstance().GetGameState() == GameManager.GameState.Prepare)
        {
            EmitSignal("TryEraseTile", xTile, yTile);
        }
    }

    public void OnMimic()
    {
        GD.Print("PlayerController: On Mimic reaction");
    }

    public void onErase()
    {
        GD.Print("PlayerController: On Erase reaction");

    }

    public void onTower()
    {
        GD.Print("PlayerController: On Tower reaction");

    }
}

