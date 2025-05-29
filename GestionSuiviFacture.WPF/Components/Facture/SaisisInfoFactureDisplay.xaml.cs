using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.ViewModels.Facture;

namespace GestionSuiviFacture.WPF.Components.Facture
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
                int tauxPercentage = 
                    string.IsNullOrEmpty(TauxTextBox.Text) 
                    ? 20 : Convert.ToInt16(TauxTextBox.Text);

                double montantHT = string.IsNullOrEmpty(MntHTTextBox.Text) 
                    ? 0 : Convert.ToDouble(MntHTTextBox.Text);

                var detail = new TaxDetail(
                    tauxPercentage,
                    montantHT
                );

                if (DataContext is FactureViewModel vm && vm.AddTaxDetailCommand.CanExecute(null))
                {
                    vm.AddTaxDetailCommand.Execute(detail);
                    vm.UpdateStatus();

                    TauxTextBox.Text = "";
                    MntHTTextBox.Text = "";
                }
                TauxTextBox.Focus();
            }
        }

        public void FocusNumFactureTextBox()
        {
            NumFactureTextBox.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // 3 decimal
            Regex regex = new Regex(@"^\d*\.?\d{0,3}$");
            e.Handled = !regex.IsMatch(fullText);
        }


        private void Taux_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // 3 decimal
            Regex regex = new Regex(@"^\d{1,3}$");
            e.Handled = !regex.IsMatch(fullText);
        }
    }
}
