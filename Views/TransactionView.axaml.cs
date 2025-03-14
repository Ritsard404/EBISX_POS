using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EBISX_POS.Models;
using EBISX_POS.ViewModels.Manager;
using System;
using System.Collections.ObjectModel;

namespace EBISX_POS.Views
{
    public partial class TransactionView : Window
    {
        public TransactionView()
        {
            InitializeComponent();
            DataContext = new TransactionViewModel();

        }
    }
}