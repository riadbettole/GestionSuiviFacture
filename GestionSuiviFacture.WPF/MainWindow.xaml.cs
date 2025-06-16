using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        //could break things
        DataContext = mainViewModel;

        mainViewModel.NavigateToFacture();
    }
}
