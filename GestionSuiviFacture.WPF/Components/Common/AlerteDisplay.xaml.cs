using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Components.Common
{
    /// <summary>
    /// Interaction logic for DetailsFacture.xaml
    /// </summary>
    public partial class AlerteDisplay : UserControl
    {
        public event EventHandler ButtonPressed;
        public AlerteDisplay()
        {
            InitializeComponent();
        }   

        public void ClosePopupAndClean()
        {
            if (DataContext is FactureViewModel vm)
            {
                vm.ClosePopupAndClean();
                ButtonPressed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnClosePopupClicked(object sender, RoutedEventArgs e)
        {
            ClosePopupAndClean(); 
        }
    }
}
