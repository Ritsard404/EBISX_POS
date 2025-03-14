using CommunityToolkit.Mvvm.ComponentModel;
using EBISX_POS.State;
using System;
using System.Diagnostics;
using System.Linq;

namespace EBISX_POS.Models
{
    public partial class TenderOrder : ObservableObject
    {
        [ObservableProperty]
        private decimal totalAmount = 0m;

        [ObservableProperty]
        private decimal tenderAmount = 0m;

        [ObservableProperty]
        private decimal discountAmount = 0m;

        [ObservableProperty]
        private decimal promoDiscountAmount = 0m;

        [ObservableProperty]
        private decimal promoDiscountPercent = 0m;

        [ObservableProperty]
        private decimal changeAmount = 0m;

        [ObservableProperty]
        private decimal amountDue = 0m;

        public decimal DiscountPwdScPercent => 0.2m;

        [ObservableProperty]
        private bool hasPromoDiscount;

        [ObservableProperty]
        private bool hasPwdScDiscount;

        [ObservableProperty]
        private bool hasOrderDiscount;

        partial void OnTotalAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnTenderAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnDiscountAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnPromoDiscountAmountChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnPromoDiscountPercentChanged(decimal oldValue, decimal newValue) => UpdateComputedValues();
        partial void OnHasPwdScDiscountChanged(bool oldValue, bool newValue) => UpdateComputedValues();

        public void Reset()
        {
            TotalAmount = 0m;
            TenderAmount = 0m;
            DiscountAmount = 0m;
            PromoDiscountAmount = 0m;
            PromoDiscountPercent = 0m;
            UpdateComputedValues();
        }

        public bool CalculateTotalAmount()
        {
            TotalAmount = OrderState.CurrentOrder.Sum(orderItem => orderItem.TotalPrice);

            if (TotalAmount <= 0)
                return true;

            return false;
        }

        private void UpdateHasPromoDiscount()
        {
            HasPromoDiscount = PromoDiscountAmount > 0 || PromoDiscountPercent > 0;
        }

        private void UpdateComputedValues()
        {
            UpdateHasPromoDiscount();
            if (HasPromoDiscount)
            {
                DiscountAmount = PromoDiscountAmount > 0
                    ? PromoDiscountAmount
                    : (PromoDiscountPercent > 0
                        ? (TotalAmount <= 500
                            ? TotalAmount * PromoDiscountPercent
                            : 500 * PromoDiscountPercent)
                        : 0m);
            }
            else if (HasPwdScDiscount)
            {
                DiscountAmount = Math.Truncate(TotalAmount * DiscountPwdScPercent * 100) / 100m;
            }
            else
            {
                DiscountAmount = 0m;
            }

            AmountDue = TotalAmount - DiscountAmount;
            ChangeAmount = TenderAmount - AmountDue;


            HasOrderDiscount = HasPromoDiscount || HasPwdScDiscount;
        }
    }
}
