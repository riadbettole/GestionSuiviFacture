using System.Windows.Controls;
using System.Windows.Input;
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

        private void DateTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DateTextBox.Focus();
        }

        private void DateInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SiteTextBox.Focus();
            }
        }
        private void SiteInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommandeTextBox.Focus();
            }
        }
        private void NumCommandeInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is FactureViewModel vm && vm.FindCommandeCommand.CanExecute(null))
                {
                    vm.FindCommandeCommand.Execute(null);
                }
                NumFactureTextBox.Focus();
            }
        }
        private void NumFactureInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MontantTTCTextBox.Focus();
            }
        }
        private void MntHTInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is FactureViewModel vm && vm.AddTaxDetailCommand.CanExecute(null))
                {
                    vm.AddTaxDetailCommand.Execute(null);
                }
                TauxTextBox.Focus();
            }
        }

        private void TauxInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MntHTTextBox.Focus();
            }
        }
        private void MontantTTCInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TauxTextBox.Focus();
            }
        }
    }
}