using Godot;
using System;
using System.Collections.Generic;
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
    private Container eBuffsBar;
    [Export]
    private Button eOpenButton;

    [Export]
    private MapObject eMap;

    [Export]
    private float eSlideTime = 0.5f;

    [Export]
    private AudioStream eCoinSound;
    [Export]
    private AudioStream eDynamiteSound;
    private List<BuffItemUsage> mAppliedBuffItems = new(2);

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
                if (PushBuff(buff, item.Icon))
                {
                    ItemUseConfirmed(item);
                }
                break;
            case LuxuryItemUsage:
                GD.Print($"You actually bought the {item.Name} for {item.Price} coins...");
                break;
        }
    }

    private async Task TryUseFieldItemAsync(FieldItemUsage fieldUsage, Item item)
    {
        GD.Print("Shop - TryUseFieldItemAsync" + fieldUsage.ToString());
        //HideMainWindow();
        eUseItemConfirmationElement.Show();
        //GameManager.GetInstance().ChangeState(GameManager.GameState.UseFieldItem);
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
        GameManager.GetInstance().ItemAtHand = null;
        GD.Print("Shop - TryUseFieldItemAsync End");
        //GameManager.GetInstance().ChangeState(GameManager.GameState.Prepare);

        eUseItemConfirmationElement.Hide();
    }

    private void ItemUseConfirmed(Item item)
    {
        GD.Print("Shop - Item Use Confirmed");
        Wallet.Balance -= item.Price;
        AudioManager.Instance.PlaySFX(eCoinSound);
        // Hardcoded Dynamite Sound. I just wanna hear the world exploding
        // https://www.youtube.com/watch?v=I685CIJkt7U&ab_channel=MoeMusic
        if (item.Name == "Dynamite")
        {
            AudioManager.Instance.PlaySFX(eDynamiteSound);
        }
        // TODO: Add coins sound here
    }

    private void ItemUseCanceled()
    {
        //ShowMainWindow();
    }

    private bool PushBuff(BuffItemUsage buff, Texture2D icon)
    {
        if (mAppliedBuffItems.Contains(buff))
        {
            return false;
        }

        mAppliedBuffItems.Add(buff);
        eBuffsBar.AddChild(new TextureRect
        {
            Texture = icon,
            ExpandMode = TextureRect.ExpandModeEnum.FitWidth
        });
        return true;
    }

    public override void _Ready()
    {
        eMainShopWindow.Position += Vector2.Left * eMainShopWindow.Size.X; //Hiding window at start
    }

    public override void _Process(double delta)
    {
        var gm = GameManager.GetInstance();

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


        if (gm.ItemAtHand is not null && Input.IsMouseButtonPressed(MouseButton.Right))
        {
            EmitSignal(SignalName.CancelItemUsage);
        }
    }

    public void OnWaveComplete()
    {
        mAppliedBuffItems.Clear();
        foreach (var child in eBuffsBar.GetChildren())
        {
            child.Free(); // The child is free now and ready to leave the nest. They grow so quickly *sob*
        }
    }


    public void OnMobSpawned(Mob mob)
    {
        mAppliedBuffItems.ForEach(buff => buff.UseAction(mob));
    }

    public void ShowOpenButton()
    {
        eOpenButton.Visible = true;
    }

    public void HideOpenButton()
    {
        eOpenButton.Visible = false;
    }
}
