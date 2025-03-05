using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace EBISX_POS.State
{
    public partial class OrderState : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<OrderItemState> _currentOrder = new();

        [ObservableProperty]
        private decimal _totalAmount;
    }
}
