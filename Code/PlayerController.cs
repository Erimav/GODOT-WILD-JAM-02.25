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


    [Export]
    // Base money each turn
    private int eMoneyEachTurn;
    [Export]
    // Money that scales with wave number
    private int eMoneyWaveBonus;
    // PUBLIC METHODS

    public void Reset()
    {
        // there is no inner state in Player Controller to reset
        // so Reset just for Conformity with other controllers
    }

    public override void _Ready()
    {
        base._Ready();
    }
    public void TilePressed(Tile tile, int xTile, int yTile)
    {
        GD.Print("PlayerController: Tile Pressed");
        GameManager gameManager = GameManager.GetInstance();
        var state = gameManager.GetGameState();
        if (gameManager.ItemAtHand is not null)
        {
            GD.Print("PlayerController: Try Use Item");
            EmitSignal(SignalName.UseItemOnTile, xTile, yTile);
        }
        else if (state == GameManager.GameState.Prepare)
        {
            EmitSignal(SignalName.TryEraseTile, xTile, yTile);
        }
    }

    public void AddMoneyToPlayer(int waveNumber)
    {
        Wallet.Balance += eMoneyEachTurn + eMoneyWaveBonus*waveNumber;
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

