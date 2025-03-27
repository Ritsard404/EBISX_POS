using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using EBISX_POS.State;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace EBISX_POS.ViewModels
{
    public partial class TenderOrderViewModel : ViewModelBase
    {
        [ObservableProperty]
        private TenderOrder tenderCurrentOrder;

        [ObservableProperty]
        private string tenderInput = "";
        public string TenderInputDisplay
        {
            get
            {
                // If the raw input ends with a decimal point, preserve it.
                if (TenderInput.EndsWith("."))
                {
                    // Remove the trailing dot temporarily.
                    string intPart = TenderInput.TrimEnd('.');
                    if (decimal.TryParse(intPart, out decimal amt))
                    {
                        // Format the integer part with thousand separators.
                        string formattedInt = amt.ToString("N0");
                        return $"₱ {formattedInt}.";
                    }
                    return $"₱ {TenderInput}";
                }
                else if (decimal.TryParse(TenderInput, out decimal amt2))
                {
                    // If there is a decimal point within the string (and not ending with one),
                    // format with two decimals; otherwise, with no decimals.
                    string format = TenderInput.Contains(".") ? "N2" : "N0";
                    string formatted = amt2.ToString(format);
                    return $"₱ {formatted}";
                }
                return $"₱ {TenderInput}";
            }
        }
        public TenderOrderViewModel()
        {
            TenderCurrentOrder = TenderState.tenderOrder;
        }

        [RelayCommand]
        private void AddPresetAmount(string content)
        {
            Debug.WriteLine($"Preset button clicked: {content}");
            if (decimal.TryParse(content, out decimal preset))
            {
                TenderCurrentOrder.TenderAmount += preset;

                // Update TenderInput to reflect the new amount with 2 decimal places.
                TenderInput = TenderCurrentOrder.TenderAmount.ToString("F2");
                // Optionally update any input string if you’re using one.
                OnPropertyChanged(nameof(TenderCurrentOrder));
                OnPropertyChanged(nameof(TenderInput));
                OnPropertyChanged(nameof(TenderInputDisplay));
                Debug.WriteLine($"New Tender Amount: {TenderCurrentOrder.TenderAmount}");
            }
            else
            {
                Debug.WriteLine("Failed to parse preset amount.");
            }
        }

        [RelayCommand]
        private void TenderButtonClick(string content)
        {
            Debug.WriteLine($"Button clicked: {content}");

            if (content == "CLEAR")
            {
                TenderInput = "";
                TenderCurrentOrder.TenderAmount = 0m;
                OnPropertyChanged(nameof(TenderInput));
                OnPropertyChanged(nameof(TenderInputDisplay));
                OnPropertyChanged(nameof(TenderCurrentOrder));
                return;
            }

            // Append the content to the raw input string.
            if (content == ".")
            {
                if (!TenderInput.Contains("."))
                {
                    TenderInput += ".";
                }
            }
            else if (content == "00")
            {
                if (TenderInput.Contains("."))
                {
                    int index = TenderInput.IndexOf(".");
                    string decimals = TenderInput.Substring(index + 1);
                    int available = 2 - decimals.Length;
                    if (available > 0)
                    {
                        TenderInput += "00".Substring(0, available);
                    }
                }
                else
                {
                    TenderInput += "00";
                }
            }
            else if (int.TryParse(content, out int _))
            {
                if (TenderInput.Contains("."))
                {
                    int index = TenderInput.IndexOf(".");
                    string decimals = TenderInput.Substring(index + 1);
                    if (decimals.Length < 2)
                    {
                        TenderInput += content;
                    }
                }
                else
                {
                    TenderInput += content;
                }
            }

            Debug.WriteLine($"Tender input string: {TenderInput}");

            if (decimal.TryParse(TenderInput, out decimal newAmount))
            {
                newAmount = Math.Round(newAmount, 2);
                TenderCurrentOrder.TenderAmount = newAmount;
            }

            // Notify the UI that both the raw input and its display have changed.
            OnPropertyChanged(nameof(TenderInput));
            OnPropertyChanged(nameof(TenderInputDisplay));
            OnPropertyChanged(nameof(TenderCurrentOrder));

            Debug.WriteLine($"Tender amount updated to: {TenderCurrentOrder.TenderAmount}");
        }
    }
}
