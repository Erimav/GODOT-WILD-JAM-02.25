using Godot;
using System;

public partial class ItemInfoDisplay : Node
{
    [Export]
    private Label eName;
    [Export]
    private Label eDescription;
    [Export]
    private ItemsGrid eItemsGrid;

    public override void _Ready()
    {
        eItemsGrid.ItemSlotSelected += OnItemSelected;
    }

    private void OnItemSelected(ItemSlot itemSlot)
    {
        var item = itemSlot.Item;
        eName.Text = item.Name;
        eDescription.Text = item.Description;
    }
}
