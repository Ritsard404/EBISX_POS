using System;

public static class CashierState
{
    private static string? _cashierName;
    public static string? CashierName
    {
        get => _cashierName;
        set
        {
            if (_cashierName != value)
            {
                _cashierName = value;
                OnCashierNameChanged?.Invoke(); // Trigger UI updates
            }
        }
    }

    // Event to notify UI
    public static event Action? OnCashierNameChanged;
}
