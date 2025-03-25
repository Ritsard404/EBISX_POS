using System.Collections.ObjectModel;
using EBISX_POS.ViewModels; // Ensure this is added
using EBISX_POS.API.Models; // Ensure this is added
using EBISX_POS.State;
using System.Threading.Tasks;
using EBISX_POS.Services;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using EBISX_POS.Models;

namespace EBISX_POS.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly MenuService _menuService;


        public ObservableCollection<Category> ButtonList { get; } = new();
        public OrderSummaryViewModel OrderSummaryViewModel { get; } // Add this property
        public ItemListViewModel ItemListViewModel { get; } // Add this property

        public MainWindowViewModel(MenuService menuService)
        {
            _menuService = menuService;
            OrderSummaryViewModel = new OrderSummaryViewModel(); // Initialize it
            ItemListViewModel = new ItemListViewModel(menuService); // Initialize it

            _ = LoadCategories();
            _ = LoadPendingOrder();


        }

        public string CashierName => CashierState.CashierName ?? "Developer";

        private async Task LoadCategories()
        {
            var categories = await _menuService.GetCategoriesAsync();
            ButtonList.Clear();
            categories.ForEach(category => ButtonList.Add(category));

            await LoadMenusAsync(categories.FirstOrDefault().Id);

        }
        private async Task LoadPendingOrder()
        {
            var orderService = App.Current.Services.GetRequiredService<OrderService>();

            // Fetch the pending orders (grouped by EntryId) from the API.
            var ordersDto = await orderService.GetCurrentOrderItems();

            // If the items collection has empty items, exit.
            if (!ordersDto.Any())
                return;

            foreach (var dto in ordersDto)
            {
                // Map the DTO's SubOrders to an ObservableCollection<SubOrderItem>
                var subOrders = new ObservableCollection<SubOrderItem>(
                    dto.SubOrders.Select(s => new SubOrderItem
                    {
                        MenuId = s.MenuId,
                        DrinkId = s.DrinkId,
                        AddOnId = s.AddOnId,
                        Name = s.Name,
                        ItemPrice = s.ItemPrice,
                        Size = s.Size,
                        Quantity = s.Quantity,
                        IsFirstItem = s.IsFirstItem,
                    })
                );

                // Create a new OrderItemState from the DTO.
                var pendingItem = new OrderItemState()
                {
                    ID = dto.EntryId,             // Using EntryId from the DTO.
                    Quantity = dto.TotalQuantity, // Total quantity from the DTO.
                    TotalPrice = dto.TotalPrice,  // Total price from the DTO.
                    HasCurrentOrder = dto.HasCurrentOrder,
                    SubOrders = subOrders,         // Mapped sub-orders.
                    HasDiscount = dto.HasDiscount,
                    IsEnableEdit = !dto.HasDiscount,
                    TotalDiscountPrice = dto.DiscountAmount,
                    IsPwdDiscounted = dto.IsPwdDiscounted,
                    IsSeniorDiscounted = dto.IsSeniorDiscounted
                };

                // Add the mapped OrderItemState to the static collection.
                OrderState.CurrentOrder.Add(pendingItem);
            }

            // Refresh UI display (if needed by your application).
            OrderState.CurrentOrderItem.RefreshDisplaySubOrders();
        }


        public async Task LoadMenusAsync(int categoryId)
        {
            await ItemListViewModel.LoadMenusAsync(categoryId);
        }
    }
}
