using Godot;
using System;

public partial class PurchaseButton : Button
{
    [Export]
    private ItemsGrid eItemsGrid;

    public override void _Ready()
    {
        eItemsGrid.ItemSlotSelected += OnItemSelected;
        Wallet.BalanceChanged += OnBalanceChanged;
    }

    private void OnBalanceChanged(int newBalance)
    {
        // More stupid null conditions
        if (eItemsGrid is not null && eItemsGrid.SelectedItemSlot is not null && eItemsGrid.SelectedItemSlot.Item is not null)
        {
            Disabled = eItemsGrid.SelectedItemSlot.Item.Price > newBalance;
        }
    }

    private void OnItemSelected(ItemSlot itemSlot)
    {
        var item = itemSlot.Item;
        Text = $"Purchase for {item.Price}";
        Disabled = item.Price > Wallet.Balance;
    }
}
