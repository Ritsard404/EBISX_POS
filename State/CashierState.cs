using System;

public static class CashierState
{
    public static event Action? OnCashierStateChanged;

    private static string? _cashierName;
    public static string? CashierName
    {
        get => _cashierName;
        set
        {
            if (_cashierName != value)
            {
                _cashierName = value;
                OnCashierStateChanged?.Invoke();
            }
        }
    }

    private static string? _cashierEmail;
    public static string? CashierEmail
    {
        get => _cashierEmail;
        set
        {
            if (_cashierEmail != value)
            {
                _cashierEmail = value;
                OnCashierStateChanged?.Invoke();
            }
        }
    }
}
