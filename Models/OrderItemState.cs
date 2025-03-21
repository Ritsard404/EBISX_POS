using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace EBISX_POS.Models
{
    public partial class OrderItemState : ObservableObject
    {
        private static int _nextId = 1;

        public long ID { get; set; }  // ID is set in constructor, no change notification needed

        [ObservableProperty]
        private int quantity;

        [ObservableProperty]
        private string? orderType;

        [ObservableProperty]
        private bool hasCurrentOrder;

        [ObservableProperty]
        private decimal totalPrice;

        [ObservableProperty]
        private decimal totalDiscountPrice;
        public bool HasDiscount { get; set; }
        public bool IsEnableEdit { get; set; } = true;

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
                    //Quantity = index == 0 ? Quantity : 1 // Only show Quantity for the first item
                    Quantity = Quantity  // Only show Quantity for the first item
                }));

        public OrderItemState()
        {
            ID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + Interlocked.Increment(ref _nextId);
            subOrders.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(DisplaySubOrders));
                UpdateTotalPrice();
            };
        }

        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(DisplaySubOrders));
            OnPropertyChanged(nameof(Quantity));
            UpdateTotalPrice();
        }

        public void RefreshDisplaySubOrders()
        {
            OnPropertyChanged(nameof(DisplaySubOrders));
            UpdateTotalPrice();
            UpdateHasCurrentOrder();
        }

        private void UpdateHasCurrentOrder()
        {
            HasCurrentOrder = subOrders.Any();
        }
        private void UpdateTotalPrice()
        {
            TotalPrice = DisplaySubOrders
            .Where(i => !(i.AddOnId == null && i.MenuId == null && i.DrinkId == null))
            .Sum(p => p.ItemSubTotal);

            TotalDiscountPrice = DisplaySubOrders
            .Where(i => (i.AddOnId == null && i.MenuId == null && i.DrinkId == null))
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

        public string DisplayName => string.IsNullOrEmpty(Size) || MenuId == null && DrinkId == null && AddOnId == null ? Name + $" @{ItemPrice.ToString("G29")}" :
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
