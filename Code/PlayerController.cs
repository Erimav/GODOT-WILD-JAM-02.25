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

    [Signal]
    public delegate void UseItemOnTileEventHandler(int xTile, int yTile);

    // PUBLIC METHODS

    public override void _Ready()
    {
        base._Ready();
    }
    public void TilePressed(Tile tile, int xTile, int yTile)
    {
        GD.Print("PlayerController: Tile Pressed");
        var state = GameManager.GetInstance().GetGameState();
        if (state == GameManager.GameState.Prepare)
        {
            EmitSignal(SignalName.TryEraseTile, xTile, yTile);
        }
        else if (state == GameManager.GameState.UseFieldItem)
        {
            GD.Print("PlayerController: Try Use Item");
            EmitSignal(SignalName.UseItemOnTile, xTile, yTile);
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

