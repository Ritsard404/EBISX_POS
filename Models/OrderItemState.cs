using System.Collections.Generic;
using System.Linq;

namespace EBISX_POS.Models
{
    public class OrderItemState
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public string? OrderType { get; set; }
        public List<SubOrderItem> SubOrders { get; set; } = new();

        // Computed property to track the first item
        public List<SubOrderItem> DisplaySubOrders => SubOrders
            .Select((s, index) => new SubOrderItem
            {
                MenuId = s.MenuId,
                Name = s.Name,
                ItemPrice = s.ItemPrice,
                Size = s.Size,
                IsFirstItem = index == 0, // True for the first item
                Quantity = index == 0 ? Quantity : 0 // Only show Quantity for the first item
            }).ToList();
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

        public string DisplayName => string.IsNullOrEmpty(Size) ? Name : $"{Name} ({Size})";

        // ✅ Opacity Property (Replaces Converter)
        public double Opacity => IsFirstItem ? 1.0 : 0.0;
    }
}
