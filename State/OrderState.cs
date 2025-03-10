using Avalonia.Controls;
using EBISX_POS.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.State
{
    public static class OrderState
    {
        public static ObservableCollection<OrderItemState> CurrentOrder { get; set; } = new ObservableCollection<OrderItemState>();
        //public static ObservableCollection<OrderItem> OrderItems = new ObservableCollection<OrderItem>();
        //public static OrderItemState CurrentOrderItem { get; set; } = new OrderItemState();

        private static OrderItemState _currentOrderItem = new OrderItemState();
        public static OrderItemState CurrentOrderItem
        {
            get => _currentOrderItem;
            set
            {
                if (_currentOrderItem != value)
                {
                    _currentOrderItem = value;
                    OnStaticPropertyChanged(nameof(CurrentOrderItem));
                }
            }
        }

        // Static event to notify when static properties change
        public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static decimal _totalAmount;


        public static void UpdateItemOrder(string itemType, int itemId, string name, decimal price, string? size)
        {
            // Determine the correct predicate based on the item type.
            Func<SubOrderItem, bool> predicate = itemType switch
            {
                "Drink" => c => c.DrinkId != null,
                "AddOn" => c => c.AddOnId != null,
                "Menu" => c => c.MenuId != null,
            };

            // Look for an existing sub-order that matches the predicate.
            var item = CurrentOrderItem.SubOrders.FirstOrDefault(predicate);

            if (item != null)
            {
                // Update the existing sub-order.
                item.Name = name;
                item.ItemPrice = price;
                item.Size = size;

                // Optionally update the ID field, if needed.
                if (itemType == "Drink")
                    item.DrinkId = itemId;
                else if (itemType == "AddOn")
                    item.AddOnId = itemId;
                else
                    item.MenuId = itemId;

                CurrentOrderItem.RefreshDisplaySubOrders();
            }
            else
            {
                // No matching sub-order exists, so create and add a new one.
                var newItem = new SubOrderItem
                {
                    Name = name,
                    ItemPrice = price,
                    Size = size
                };

                if (itemType == "Drink")
                    newItem.DrinkId = itemId;
                else if (itemType == "AddOn")
                    newItem.AddOnId = itemId;
                else
                    newItem.MenuId = itemId;

                CurrentOrderItem.SubOrders.Add(newItem);
            }
        }

        public static void DisplayOrders()
        {
            var order = CurrentOrderItem;

            Debug.WriteLine($"🛒 Current Order - ID: {order.ID}, Type: {order.OrderType}, Quantity: {order.Quantity}");
            Debug.WriteLine("----------------------------------------------------");

            foreach (var subOrder in order.DisplaySubOrders)
            {
                Debug.WriteLine($"📌 Item: {subOrder.DisplayName}");
                Debug.WriteLine($"   🔹 Price: {subOrder.ItemPrice:C}");
                Debug.WriteLine($"   🔹 Quantity: {subOrder.Quantity}");
                Debug.WriteLine($"   🔹 Subtotal: {subOrder.ItemSubTotal:C}");
                Debug.WriteLine($"   🔹 Size: {subOrder.Size}");
                Debug.WriteLine($"   🔹 IsFirstItem: {subOrder.IsFirstItem}");
                Debug.WriteLine("----------------------------------------------------");
            }
        }

        public static bool FinalizeCurrentOrder()
        {
            var isNoDrinks = CurrentOrderItem.SubOrders
                .All(s => s.DrinkId == null);
            if (isNoDrinks)
            {
                var alertBox = MessageBoxManager.GetMessageBoxStandard(
                    new MessageBoxStandardParams
                    {
                        ContentTitle = "Required Drink!",
                        ContentMessage = "Please select a drink.",
                        ButtonDefinitions = ButtonEnum.Ok, // Defines the available buttons
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        CanResize = false,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        Width = 400,
                        ShowInCenter = true
                    }); 
                

                alertBox.ShowAsync();

                return false;
            };

            // Add the current order item to the collection
            CurrentOrder.Add(CurrentOrderItem);

            // Reset the current order item to a new instance for the next order
            CurrentOrderItem = new OrderItemState();

            // Optionally, notify any subscribers that the current order item has changed
            OnStaticPropertyChanged(nameof(CurrentOrderItem));

            return true;
        }

        //private static void InitialCurrentOrder(int orderId, int itemId)
        //{
        //    var orderList = CurrentOrder.FirstOrDefault(i => i.ID == orderId);
        //    if (orderList != null)
        //    {
        //        CurrentOrder.Add(CurrentOrderItem);
        //    }
        //    else
        //    {
        //        var subOrderList = orderList.;
        //    }
        //}

    }
}
