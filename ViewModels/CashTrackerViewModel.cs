using EBISX_POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.ViewModels
{
    public class CashTrackerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<InvoiceReceipt> _cashTrack;
        public ObservableCollection<InvoiceReceipt> CashTrack
        {
            get => _cashTrack;
            set
            {
                _cashTrack = value;
                OnPropertyChanged(nameof(CashTrack));
            }
        }

        public CashTrackerViewModel() {
            CashTrack = new ObservableCollection<InvoiceReceipt>
            {
                new InvoiceReceipt()
                {
                    // Transaction Summary
                    CashInDrawer = 720.00m,
                    WithdrawalAmount = 2300.00m,
                    ShortOver = 1.60m
                }
            };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
