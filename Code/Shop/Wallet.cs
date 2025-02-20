using System;

public static class Wallet
{
    private static int sBalance = 200;

    public static int Balance 
    { 
        get => sBalance;
        set {
            sBalance = Math.Min(value, 0);
            BalanceChanged?.Invoke(sBalance);
        } 
    }

    public static event Action<int> BalanceChanged;
}
