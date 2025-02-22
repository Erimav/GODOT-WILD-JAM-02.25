using Godot;
using System;

public partial class ItemSlot : Node
{
    [Export]
    private Texture2D eNormalFrameTexture;
    [Export]
    private Texture2D eSelectedFrameTexture;

    [Export]
    private TextureRect eFrame;
    [Export]
    private TextureButton eButton;

    private ItemsGrid mItemsGrid;

    public ItemsGrid ItemsGrid
    {
        get => mItemsGrid; 
        set
        {
            mItemsGrid = value;
            mItemsGrid.ItemSlotSelected += OnItemSelected;
        }
    }
    public Item Item { get; set; }
    
    private void OnItemClicked()
    {
        ItemsGrid.SelectSlot(this);
    }

    private void OnItemSelected(ItemSlot slot)
    {
        var frameTex = slot == this
            ? eSelectedFrameTexture
            : eNormalFrameTexture;

        eFrame.Texture = frameTex;
    }

    public override void _Ready()
    {
        eButton.Pressed += OnItemClicked;
        eButton.TextureNormal = Item.Icon;
    }
}
