using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EBISX_POS.Models;
using EBISX_POS.Services;
using Tmds.DBus.Protocol;

namespace EBISX_POS.ViewModels
{
    public partial class CrewLogEntryViewModel : ObservableObject
    {
        private readonly CrewServices _crewService;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private CrewMember? _selectedCrew;
        

        public ObservableCollection<CrewMember> CrewMembers { get; }

        public CrewLogEntryViewModel(CrewServices crewService)
        {
            _crewService = crewService;
            CrewMembers = new ObservableCollection<CrewMember>(_crewService.GetAllCrewMembers());
        }

        [RelayCommand]
        private void Login()
        {
            if (SelectedCrew != null)
            {
                var crewMember = _crewService.Authenticate(SelectedCrew.CrewMemberId);
                Message = crewMember != null ? $"Welcome, {crewMember.Name}!" : "Crew ID not found. Try again.";
            }
        }
    }
}
