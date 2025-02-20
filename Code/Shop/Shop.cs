using Godot;
using System;

public partial class Shop : Control
{
    [Export]
    private Control eMainShopWindow;
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

    public override void _Ready()
    {   
        eMainShopWindow.Position += Vector2.Left * eMainShopWindow.Size.X; //Hiding window at start
    }

    public override void _Process(double delta)
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
}
