using Godot;
using System;

public partial class PurchaseButton : Button
{
    [Export]
    private ItemsGrid eItemsGrid;

    public override void _Ready()
    {
        eItemsGrid.ItemSlotSelected += OnItemSelected;
        Pressed += TryPurchase;
    }

    private void TryPurchase()
    {
        throw new NotImplementedException();
    }

    private void OnItemSelected(ItemSlot itemSlot)
    {
        var item = itemSlot.Item;
        Text = item.Price.ToString();
        Disabled = item.Price <= Wallet.Balance;
    }
}
