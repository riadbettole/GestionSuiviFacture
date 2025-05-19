using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestionSuiviFacture.WPF.Components;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Facture : UserControl
    {
        public Facture()
        {
            InitializeComponent();
        }

        private void DateTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            DateTextBox.Focus();
        }

        private void NumCommandeInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is FactureViewModel vm && vm.FindCommandeCommand.CanExecute(null))
                {
                    vm.FindCommandeCommand.Execute(null);
                }
                SaisisInfoFactureDisplay.FocusNumFactureTextBox();
            }
        }

        private void DateInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SiteTextBox.Focus();
            }
        }
        private void SiteInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommandeTextBox.Focus();
            }
        }
      
     

        
        
    }
}