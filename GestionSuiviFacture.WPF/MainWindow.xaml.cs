using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var mainViewModel = new MainViewModel();
        DataContext = mainViewModel;

        mainViewModel.NavigateToFacture();
    }
}
