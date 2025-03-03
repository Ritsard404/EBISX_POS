using CommunityToolkit.Mvvm.ComponentModel;

namespace EBISX_POS.State
{
    public partial class CashierState: ObservableObject
    {
        [ObservableProperty]
        private string _cashierName;
    }
}
