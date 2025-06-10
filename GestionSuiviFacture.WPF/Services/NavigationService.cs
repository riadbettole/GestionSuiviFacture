using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.Services
{
    public class NavigationService : ObservableObject
    {
        private ObservableObject? _currentViewModel;
        public ObservableObject? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                //Simple cleanup for the old ViewModel before replacing it
                if (_currentViewModel is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                SetProperty(ref _currentViewModel, value);
            }
        }

        public void NavigateTo(ObservableObject viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
