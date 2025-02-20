using Godot;
using System;

public partial class ItemsGrid : Node
{
    [Signal]
    public delegate void ItemSlotSelectedEventHandler(ItemSlot itemSlot);

    [Export]
    private PackedScene eItemSlotPrefab;

    public ItemSlot SelectedItemSlot { get; set; }

    public void SelectSlot(ItemSlot itemSlot)
    {
        SelectedItemSlot = itemSlot;
        EmitSignal(SignalName.ItemSlotSelected, itemSlot);
    }

    public override void _Ready()
    {
        ItemSlot firstItemSlot = null;
        foreach(var item in Item.List)
        {
            var slot = eItemSlotPrefab.Instantiate<ItemSlot>();
            slot.Item = item;
            slot.ItemsGrid = this;
            CallDeferred("add_child", slot);
            firstItemSlot ??= slot;
        }

        CallDeferred(nameof(SelectSlot), firstItemSlot);
    }
}
