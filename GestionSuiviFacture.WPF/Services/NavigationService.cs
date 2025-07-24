using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.Services;

public class NavigationService : ObservableObject
{
    private ObservableObject? _currentViewModel;
    public ObservableObject? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public void NavigateTo(ObservableObject viewModel)
    {
        CurrentViewModel = viewModel;
    }
}
