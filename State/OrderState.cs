using EBISX_POS.Models;
using System.Collections.ObjectModel;

namespace EBISX_POS.State
{
    public static class OrderState
    {
        public static ObservableCollection<OrderItemState> CurrentOrder { get; set; } = new ObservableCollection<OrderItemState>();
        //public static ObservableCollection<OrderItem> OrderItems = new ObservableCollection<OrderItem>();
        public static OrderItemState CurrentOrderItem { get; set; } = new OrderItemState();
        public static decimal _totalAmount;



    }
}
