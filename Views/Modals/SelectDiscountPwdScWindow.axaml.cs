using Avalonia.Controls;
using Avalonia.Interactivity;
using EBISX_POS.API.Services.DTO.Order;
using EBISX_POS.Models;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.Views
{
    public partial class SelectDiscountPwdScWindow : Window, INotifyPropertyChanged
    {
        // Backing field for a single selection.
        // false means Senior is selected (the default), true means PWD is selected.
        private bool _isPwdSelected = false;

        // Public property for PWD selection.
        public bool IsPwdSelected
        {
            get => _isPwdSelected;
            set
            {
                if (_isPwdSelected != value)
                {
                    _isPwdSelected = value;
                    OnPropertyChanged(nameof(IsPwdSelected));
                    // Also update the complementary property.
                    OnPropertyChanged(nameof(IsSeniorSelected));
                }
            }
        }

        // Derived property: true when Senior is selected (i.e. when IsPwdSelected is false).
        public bool IsSeniorSelected
        {
            get => !_isPwdSelected;
            set
            {
                // When binding sets IsSeniorSelected, update IsPwdSelected accordingly.
                // If Senior is set to true, then PWD is false, and vice versa.
                if (value != (!_isPwdSelected))
                {
                    IsPwdSelected = !value;
                }
            }
        }
        private int _maxSelectionCount = 0;
        public int MaxSelectionCount
        {
            get => _maxSelectionCount;
            set
            {
                if (_maxSelectionCount != value)
                {
                    _maxSelectionCount = value;
                    OnPropertyChanged(nameof(MaxSelectionCount));
                }
            }
        }

        // New property for the total selected count.
        private int _totalSelected;
        public int TotalSelected
        {
            get => _totalSelected;
            set
            {
                if (_totalSelected != value)
                {
                    _totalSelected = value;
                    OnPropertyChanged(nameof(TotalSelected));
                }
            }
        }

        // New property for the selected IDs.
        private List<string> _selectedIDs = new List<string>();
        public List<string> SelectedIDs
        {
            get => _selectedIDs;
            set
            {
                if (_selectedIDs != value)
                {
                    _selectedIDs = value;
                    OnPropertyChanged(nameof(SelectedIDs));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SelectDiscountPwdScWindow()
        {

            InitializeComponent();
            DataContext = this;
            this.Opened += OnWindowOpened;
        }

        private void OnWindowOpened(object? sender, System.EventArgs e)
        {
            RefreshCurrentOrder();
        }

        private void RefreshCurrentOrder()
        {
            CurrentOrder.ItemsSource = OrderState.CurrentOrder
                .Where(d => !d.HasDiscount)
                .GroupBy(i => i.ID)
                .Select(g => g.First());
        }

        private void EditQuantity_Click(object? sender, RoutedEventArgs e)
        {
            // Retrieve the CommandParameter from the sender (a Button in this example).
            // Alternatively, if you attach the parameter directly from XAML, you can also use e.Parameter.
            if (sender is Button button && button.CommandParameter is object parameter)
            {
                int intDelta = 0;

                // If parameter is an integer.
                if (parameter is int directValue)
                {
                    intDelta = directValue;
                }
                // If it's a string, try to parse it.
                else if (parameter is string s && int.TryParse(s, out int parsedValue))
                {
                    intDelta = parsedValue;
                }
                else
                {
                    return;
                }


                // Calculate the new max selection count.
                int newMax = MaxSelectionCount + intDelta;
                if (newMax < 0)
                    newMax = 0;  // Ensure it doesn't go negative

                // Clamp newMax so that it doesn't exceed the total available items.
                int totalAvailable = OrderState.CurrentOrder.Count;
                if (newMax > totalAvailable)
                    newMax = totalAvailable;

                // If the new max is less than the current total selected, remove the excess selections.
                if (newMax < TotalSelected)
                {
                    // Calculate how many items need to be removed.
                    int excessCount = TotalSelected - newMax;
                    // Remove items from the SelectedItems collection.
                    var itemsToRemove = CurrentOrder.SelectedItems
                                        .Cast<OrderItemState>()
                                        .Take(excessCount)
                                        .ToList();
                    foreach (var item in itemsToRemove)
                    {
                        CurrentOrder.SelectedItems.Remove(item);
                    }
                }

                // Finally, update the MaxSelectionCount property.
                MaxSelectionCount = newMax;
            }
        }

        // This event is triggered whenever the selection changes in the ListBox.
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
                return;

            // If the selected items exceed the allowed count, remove extra items.
            if (listBox.SelectedItems.Count > MaxSelectionCount)
            {
                // Using ToList to iterate safely over a copy
                foreach (var added in e.AddedItems)
                {
                    if (listBox.SelectedItems.Count <= MaxSelectionCount)
                        break;
                    listBox.SelectedItems.Remove(added);
                }
            }

            // Get the total selected count.
            TotalSelected = listBox.SelectedItems.Count;

            // Extract the IDs from the selected items.
            SelectedIDs = listBox.SelectedItems
                .Cast<OrderItemState>()
                .Select(item => item.ID)
                .ToList();
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (SelectedIDs.Count() == 0)
            {
                Close();
                return;
            }
            if (SelectedIDs.Count() > MaxSelectionCount || SelectedIDs.Count() < MaxSelectionCount)
            {
                return;
            }

            Debug.WriteLine("SelectedIDs: " + string.Join(", ", SelectedIDs));

            var orderService = App.Current.Services.GetRequiredService<OrderService>();


            await orderService.AddPwdScDiscount(new AddPwdScDiscountDTO()
            {
                EntryId = SelectedIDs,
                ManagerEmail = "qwee",
                PwdScCount = MaxSelectionCount,
                IsSeniorDisc = !IsPwdSelected
            }); // Fetch the pending orders (grouped by EntryId) from the API.

            var ordersDto = await orderService.GetCurrentOrderItems();

            // If the items collection has empty items, exit.
            if (!ordersDto.Any())
                return;

            OrderState.CurrentOrder.Clear();
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
                    SubOrders = subOrders,
                    HasDiscount = dto.HasDiscount,// Mapped sub-orders.
                    TotalDiscountPrice = dto.DiscountAmount,
                    IsPwdDiscounted = dto.IsPwdDiscounted,
                    IsSeniorDiscounted = dto.IsSeniorDiscounted,
                    PromoDiscountAmount = dto.PromoDiscountAmount,
                    HasPwdScDiscount = dto.HasDiscount && dto.PromoDiscountAmount == null,
                    CouponCode = dto.CouponCode

                };

                // Add the mapped OrderItemState to the static collection.
                OrderState.CurrentOrder.Add(pendingItem);
            }

            // Refresh UI display (if needed by your application).
            OrderState.CurrentOrderItem.RefreshDisplaySubOrders();

            Close();
        }
    }
}
