using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.State;
using System;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.Models
{
    public partial class TenderOrder : ObservableObject
    {
        [ObservableProperty] private decimal totalAmount = 0m;
        [ObservableProperty] private decimal tenderAmount = 0m;
        [ObservableProperty] private decimal discountAmount = 0m;
        [ObservableProperty] private decimal promoDiscountAmount = 0m;
        [ObservableProperty] private decimal promoDiscountPercent = 0m;
        [ObservableProperty] private decimal changeAmount = 0m;
        [ObservableProperty] private decimal amountDue = 0m;
        [ObservableProperty] private bool hasPromoDiscount;
        [ObservableProperty] private bool hasCouponDiscount;
        [ObservableProperty] private bool hasPwdDiscount;
        [ObservableProperty] private bool hasScDiscount;
        [ObservableProperty] private bool hasOrderDiscount;
        public string OrderType { get; set; } = "";

        // Trigger recalculations when key properties change
        partial void OnTotalAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnTenderAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnDiscountAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnPromoDiscountAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnPromoDiscountPercentChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnHasPwdDiscountChanged(bool oldValue, bool newValue) => UpdateComputedValues();
        partial void OnHasScDiscountChanged(bool oldValue, bool newValue) => UpdateComputedValues();
        partial void OnHasPromoDiscountChanged(bool oldValue, bool newValue) => UpdateComputedValues();
        partial void OnHasCouponDiscountChanged(bool oldValue, bool newValue) => UpdateComputedValues();

        public void Reset()
        {
            TotalAmount = TenderAmount = DiscountAmount = PromoDiscountAmount = PromoDiscountPercent = 0m;
            HasPromoDiscount = HasScDiscount = HasPwdDiscount = HasOrderDiscount = false;
            UpdateComputedValues();
        }

        public bool CalculateTotalAmount()
        {
            TotalAmount = OrderState.CurrentOrder
                .Sum(orderItem => orderItem.TotalPrice);
            UpdateComputedValues();
            return TotalAmount <= 0;
        }

        private void UpdateComputedValues()
        {
            HasPromoDiscount = PromoDiscountAmount > 0 || PromoDiscountPercent > 0;
            HasCouponDiscount = OrderState.CurrentOrder
                .Any(orderItem => orderItem.CouponCode != null);
            HasOrderDiscount = HasPromoDiscount || HasScDiscount || HasPwdDiscount || HasCouponDiscount;

            if (HasPromoDiscount)
            {
                DiscountAmount = PromoDiscountAmount;
            }
            else if (HasPwdDiscount || HasScDiscount)
            {
                DiscountAmount = OrderState.CurrentOrder
                        .Sum(orderItem => orderItem.TotalDiscountPrice);

            }
            else
            {
                DiscountAmount = 0m;
            }

            if (PromoDiscountAmount >= TotalAmount)
            {
                AmountDue = 0;
                ChangeAmount = 0;
            }
            else
            {
                AmountDue = TotalAmount - DiscountAmount;
                ChangeAmount = TenderAmount - AmountDue;
            }
        }
    }
}
