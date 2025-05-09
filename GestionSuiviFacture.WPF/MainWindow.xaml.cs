using System.Windows;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}