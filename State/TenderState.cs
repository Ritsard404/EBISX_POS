using System;

namespace EBISX_POS.State
{
    public static class TenderState
    {
        public static decimal TotalAmount { get; set; } = 0m;
        public static decimal TenderAmount { get; set; } = 0m;
        public static decimal DiscountAmount { get; set; } = 0m;
        public static decimal PromoDiscountAmount { get; set; } = 0m;
        public static decimal PromoDiscountPercent { get; set; } = 0m;

        // Fixed 20% discount for SC/PWD
        public static decimal DiscountPwdScPercent => 0.2m;

        // Dynamically calculate Promo Discount
        public static decimal PromoDiscount
        {
            get
            {
                if (PromoDiscountAmount > 0) return PromoDiscountAmount; // Fixed discount if set
                if (PromoDiscountPercent <= 0) return 0m; // No discount if percent is 0
                return TotalAmount <= 500 ? TotalAmount * PromoDiscountPercent : 500 * PromoDiscountPercent;
            }
        }

        // Automatically calculate change
        public static decimal ChangeAmount
        {
            get => TenderAmount - ((TotalAmount - DiscountAmount - PromoDiscount));
        }

        /// <summary>
        /// Resets all tender state values to defaults.
        /// </summary>
        public static void Reset()
        {
            TotalAmount = 0m;
            TenderAmount = 0m;
            DiscountAmount = 0m;
            PromoDiscountAmount = 0m;
            PromoDiscountPercent = 0m;
        }
    }
}
