using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        public NavigationService Navigation { get; }

        public MainViewModel()
        {
            Navigation = new NavigationService();
            Navigation.NavigateTo(new ConsultationViewModel());
        }

        [RelayCommand]
        private void NavigateToFacture() => Navigation.NavigateTo(new FactureViewModel());

        [RelayCommand]
        private void NavigateToFactureEmballage() => Navigation.NavigateTo(new FactureEmballageViewModel());

        [RelayCommand]
        private void NavigateToConsultation() => Navigation.NavigateTo(new ConsultationViewModel());

        public void Dispose()
        {
            if (Navigation.CurrentViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }
    }
}
