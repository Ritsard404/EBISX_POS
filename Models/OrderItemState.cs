using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace EBISX_POS.Models
{
    public partial class OrderItemState : ObservableObject
    {
        private static long _counter = 0;
        public string ID { get; set; }  // ID is set in constructor, no change notification needed

        [ObservableProperty]
        private int quantity;

        [ObservableProperty]
        private string? orderType;

        [ObservableProperty]
        private bool hasCurrentOrder;

        [ObservableProperty]
        private decimal totalPrice;

        public decimal TotalDiscountPrice { get; set; }
        public bool HasDiscount { get; set; }
        public bool IsEnableEdit { get; set; } = true;
        public bool IsPwdDiscounted { get; set; } = false;
        public bool IsSeniorDiscounted { get; set; } = false;

        // Using ObservableCollection so UI is notified on add/remove.
        [ObservableProperty]
        private ObservableCollection<SubOrderItem> subOrders = new ObservableCollection<SubOrderItem>();

        // Computed property: UI must be notified manually if you need it to update dynamically.
        public ObservableCollection<SubOrderItem> DisplaySubOrders =>
            new ObservableCollection<SubOrderItem>(subOrders
                .Select((s, index) => new SubOrderItem
                {
                    MenuId = s.MenuId,
                    DrinkId = s.DrinkId,
                    AddOnId = s.AddOnId,
                    Name = s.Name,
                    ItemPrice = s.ItemPrice,
                    Size = s.Size,
                    IsFirstItem = index == 0, // True for the first item
                    Quantity = Quantity  // Only show Quantity for the first item
                }));

        public OrderItemState()
        {
            RegenerateID();
            subOrders.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(DisplaySubOrders));
                UpdateTotalPrice();
            };
        }

        // Call this to generate a new unique ID.
        public void RegenerateID()
        {
            long ticks = DateTime.UtcNow.Ticks; // high resolution
            long count = Interlocked.Increment(ref _counter);
            ID = $"{ticks}-{Guid.NewGuid().ToString()}-{count}";
        }

        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(DisplaySubOrders));
            OnPropertyChanged(nameof(Quantity));
            UpdateTotalPrice();
        }

        public void RefreshDisplaySubOrders(bool regenerateId = false)
        {
            if (regenerateId)
            {
                RegenerateID();
            }

            OnPropertyChanged(nameof(DisplaySubOrders));
            UpdateTotalPrice();
            UpdateHasCurrentOrder();
        }

        private void UpdateHasCurrentOrder()
        {
            HasCurrentOrder = SubOrders.Any();
        }
        private void UpdateTotalPrice()
        {
            TotalPrice = DisplaySubOrders
            .Where(i => !(i.AddOnId == null && i.MenuId == null && i.DrinkId == null))
            .Sum(p => p.ItemSubTotal);

        }

    }

    public class SubOrderItem
    {
        public int? MenuId { get; set; }
        public int? DrinkId { get; set; }
        public int? AddOnId { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal ItemPrice { get; set; } = 0;
        public decimal ItemSubTotal => ItemPrice * Quantity;
        public string? Size { get; set; }

        public bool IsFirstItem { get; set; } = false;
        public int Quantity { get; set; } = 0; // Store Quantity for first item

        public string DisplayName => string.IsNullOrEmpty(Size) || MenuId == null && DrinkId == null && AddOnId == null ? Name :
            ItemPrice > 0 ? Name + $" ({Size}) @{ItemPrice.ToString("G29")}" :
            $"{Name} ({Size})";

        // Opacity Property (replaces a converter)
        public double Opacity => IsFirstItem ? 1.0 : 0.0;

        public bool IsUpgradeMeal => ItemPrice > 0;

        public string ItemPriceString => IsFirstItem ? "₱" + ItemSubTotal.ToString("G29")
            : MenuId == null && DrinkId == null && AddOnId == null ? "- ₱" + ItemSubTotal.ToString("G29")
            : IsUpgradeMeal ? "+ ₱" + ItemSubTotal.ToString("G29")
            : "";
    }
}
