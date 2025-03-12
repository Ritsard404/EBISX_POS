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
using System.Threading.Tasks;

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
                    OnStaticPropertyChanged(nameof(CurrentOrderItem.Quantity));
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
                CurrentOrderItem.RefreshDisplaySubOrders();
            }

        }

        public static async Task<bool> FinalizeCurrentOrder(bool isSolo, Window owner)
        {
            var isNoDrinks = CurrentOrderItem.SubOrders
                .All(s => s.DrinkId == null);
            var isNoAddOn = CurrentOrderItem.SubOrders
                .All(s => s.AddOnId == null);

            if (!isSolo && (isNoDrinks || isNoAddOn))
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    new MessageBoxStandardParams
                    {
                        ContentHeader = "Required Drink/Side!",
                        ContentMessage = "Please select a drink/side.",
                        ButtonDefinitions = ButtonEnum.Ok, // Defines the available buttons
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        CanResize = false,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        Width = 400,
                        ShowInCenter = true,
                        Icon = Icon.Info
                    });


                var result = await box.ShowAsPopupAsync(owner);
                switch (result)
                {
                    case ButtonResult.Ok:
                        return false;
                    default:
                        return false;
                };

            };

            // Add the current order item to the collection
            CurrentOrder.Add(CurrentOrderItem);

            // Reset the current order item to a new instance for the next order\
            CurrentOrderItem = new OrderItemState();

            // Optionally, notify any subscribers that the current order item has changed
            CurrentOrderItem.RefreshDisplaySubOrders();
            OnStaticPropertyChanged(nameof(CurrentOrderItem));
            OnStaticPropertyChanged(nameof(CurrentOrder));

            return true;
        }

        public static void VoidCurrentOrder(OrderItemState orderItem)
        {
            var voidOrder = CurrentOrder.FirstOrDefault(i => i.ID == orderItem.ID);
            CurrentOrder.Remove(voidOrder);

            OnStaticPropertyChanged(nameof(CurrentOrder));
        }
    }
}
