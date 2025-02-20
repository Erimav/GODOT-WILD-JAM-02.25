using Godot;
using System;

public partial class BalanceDisplay : Label
{
    public override void _Ready()
    {
        OnBalanceChanged(Wallet.Balance);
        Wallet.BalanceChanged += OnBalanceChanged;
    }

    private void OnBalanceChanged(int balance) => Text = balance.ToString();

    public override void _ExitTree()
    {
        Wallet.BalanceChanged -= OnBalanceChanged;
    }
}
