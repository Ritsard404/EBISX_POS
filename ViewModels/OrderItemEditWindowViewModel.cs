using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using System.Diagnostics;

namespace EBISX_POS.ViewModels
{
    public partial class OrderItemEditWindowViewModel : ViewModelBase
    {
        public OrderItemState OrderItem { get; }
        public OrderItemEditWindowViewModel(OrderItemState orderItem)
        {
            OrderItem = orderItem;

        }

        // Using object as parameter to safely convert the parameter value.
        [RelayCommand]
        private void EditQuantity(object delta)
        {
            int intDelta = 0;

            // If delta is an integer directly.
            if (delta is int directValue)
            {
                intDelta = directValue;
            }
            // If it's coming as a string (common in XAML bindings).
            else if (delta is string s && int.TryParse(s, out int parsedValue))
            {
                intDelta = parsedValue;
            }
            else
            {
                Debug.WriteLine("EditQuantity received an invalid parameter.");
                return;
            }

            // Ensure quantity does not fall below 1.
            if (OrderItem.Quantity + intDelta >= 1)
            {
                OrderItem.Quantity += intDelta;
                Debug.WriteLine($"Quantity updated to: {OrderItem.Quantity}");
            }
        }
    }
}
