using EBISX_POS.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace EBISX_POS.State
{
    public static class OrderState
    {
        public static ObservableCollection<OrderItemState> CurrentOrder { get; set; } = new ObservableCollection<OrderItemState>();
        //public static ObservableCollection<OrderItem> OrderItems = new ObservableCollection<OrderItem>();
        public static OrderItemState CurrentOrderItem { get; set; } = new OrderItemState();
        public static decimal _totalAmount;

        public static void UpdateDrinksOrder(int drinkId, string name, decimal price, string size)
        {
            var drink = CurrentOrderItem.SubOrders.FirstOrDefault(c => c.DrinkId == drinkId);
            if (drink != null)
            {
                drink.Name = name;
                drink.ItemPrice = price;
                drink.Size = size;
            }
            else
            {

                CurrentOrderItem.SubOrders.Add(new SubOrderItem
                {
                    DrinkId = drinkId,
                    Name = name,
                    ItemPrice = price,
                    Size = size
                });
            }
        }

    }
}
