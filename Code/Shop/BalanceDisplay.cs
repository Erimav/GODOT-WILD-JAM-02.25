using Godot;
using System;

public partial class BalanceDisplay : Label
{
    private int mLastBalance;

    public override void _Ready()
    {
        mLastBalance = Wallet.Balance;
        Text = Wallet.Balance.ToString();
        Wallet.BalanceChanged += OnBalanceChanged;
    }

    private void OnBalanceChanged(int balance)
    {
        Text = balance.ToString();
        CreateDiffLabel(balance - mLastBalance);
        mLastBalance = balance;
    }

    public override void _ExitTree()
    {
        Wallet.BalanceChanged -= OnBalanceChanged;
    }

    private void CreateDiffLabel(int diff)
    {
        if (diff == 0)
            return;

        var text = diff > 0 ? $"+{diff}" : diff.ToString();

        var label = new Label
        {
            Text = text,
            Theme = Theme
        };

        var color = diff > 0 ? Color.Color8(0, 255, 0) : Color.Color8(255, 0, 0);
        label.AddThemeColorOverride("font_color", color);
        GetTree().Root.AddChild(label);
        label.GlobalPosition = GlobalPosition + Vector2.Down * 10;
        label.Size = Size;

        var tween = CreateTween();
        tween.TweenProperty(label, "position:y", label.Position.Y + 40, 1f);
        tween.TweenCallback(Callable.From(() => label.Free()));
    }
}
