using Godot;
using System;
using System.Threading.Tasks;

public partial class Shop : Control
{
    [Signal]
    public delegate void CancelItemUsageEventHandler();

    [Export]
    private Control eMainShopWindow;
    [Export]
    private Control eUseItemConfirmationElement;
    [Export]
    private ItemsGrid eItemsGrid;

    [Export]
    private MapObject eMap;

    [Export]
    private float eSlideTime = 0.5f;

    public bool IsOpen { get; private set; }

    public void ShowMainWindow()
    {
        if (IsOpen)
        {
            return;
        }
        IsOpen = true;
        SlideHorizontalyTo(0);
    }

    public void HideMainWindow()
    {
        if (!IsOpen)
        {
            return;
        }
        IsOpen = false;
        SlideHorizontalyTo(-eMainShopWindow.Size.X);
    }

    private void SlideHorizontalyTo(float x)
    {
        GD.Print($"Sliding to {x}");
        var tween = CreateTween();
        tween.TweenProperty(eMainShopWindow, "position:x", x, eSlideTime)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Quad);
    }

    private void TryUseItem()
    {
        var item = eItemsGrid.SelectedItemSlot.Item;

        switch (item.Usage)
        {
            case FieldItemUsage fieldUsage:
                _ = TryUseFieldItemAsync(fieldUsage, item);
                break;
            case BuffItemUsage buff:
                break;
            case LuxuryItemUsage:
                GD.Print($"You actually bought the {item.Name} for {item.Price} coins...");
                break;
        }
    }

    private async Task TryUseFieldItemAsync(FieldItemUsage fieldUsage, Item item)
    {
        //HideMainWindow();
        eUseItemConfirmationElement.Show();
        GameManager.GetInstance().ChangeState(GameManager.GameState.UseFieldItem);
        GameManager.GetInstance().ItemAtHand = item;
        var signalToAwait = fieldUsage.UseAction switch
        {
            FieldItemUsage.Action.Reveal => MapObject.SignalName.TileRevealed,
            FieldItemUsage.Action.BlowUp => MapObject.SignalName.TileBlownUp,
            _ => throw new IndexOutOfRangeException(),
        };
        var waitForConfirmationTask = ToSignal(eMap, signalToAwait).AsTask();
        var waitForCancelTask = ToSignal(this, SignalName.CancelItemUsage).AsTask();
        var result = await Task.WhenAny(waitForConfirmationTask, waitForCancelTask);
        if (result == waitForConfirmationTask)
        {
            ItemUseConfirmed(item);
        } 
        else
        {
            ItemUseCanceled();
        }
        GameManager.GetInstance().ChangeState(GameManager.GameState.Prepare);

        eUseItemConfirmationElement.Hide();
    } 

    private void ItemUseConfirmed(Item item)
    {
        Wallet.Balance -= item.Price;
        // TODO: Add coins sound here
    }

    private void ItemUseCanceled()
    {
        //ShowMainWindow();
    }

    public override void _Ready()
    {
        eMainShopWindow.Position += Vector2.Left * eMainShopWindow.Size.X; //Hiding window at start
    }

    public override void _Process(double delta)
    {
        var state = GameManager.GetInstance().GetGameState();
        if (state == GameManager.GameState.Prepare)
        {
            if (Input.IsActionJustPressed("shop"))
            {
                if (IsOpen)
                {
                    HideMainWindow();
                }
                else
                {
                    ShowMainWindow();
                }
            }
        }

        if (state == GameManager.GameState.UseFieldItem && Input.IsMouseButtonPressed(MouseButton.Right))
        {
            EmitSignal(SignalName.CancelItemUsage);
        }
    }
}
