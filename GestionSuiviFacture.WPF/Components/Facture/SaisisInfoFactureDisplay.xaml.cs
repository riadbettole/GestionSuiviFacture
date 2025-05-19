using System.Windows.Controls;
using System.Windows.Input;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Components
{
    /// <summary>
    /// Interaction logic for SaisisInfoFactureDisplay.xaml
    /// </summary>
    public partial class SaisisInfoFactureDisplay : UserControl
    {
        public SaisisInfoFactureDisplay()
        {
            InitializeComponent();
        }


        private void NumFactureInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MontantTTCTextBox.Focus();
            }
        }

        private void MontantTTCInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
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

        public void FocusNumFactureTextBox()
        {
            NumFactureTextBox.Focus();
        }
    }
}
