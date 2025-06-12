using GestionSuiviFacture.WPF.ViewModels;
using GestionSuiviFacture.WPF.ViewModels.Helpers;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Components.Facture;

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

    private void AddFactureButton_LeftClick(object sender, RoutedEventArgs e)
    {
        AddLigneFacture();
    }

    private void MntHTInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddLigneFacture();
        }
    }

    private void AddLigneFacture()
    {
        int tauxPercentage = string.IsNullOrEmpty(TauxTextBox.Text)
            ? 20
            : Convert.ToInt16(TauxTextBox.Text);

        double montantHT = string.IsNullOrEmpty(MntHTTextBox.Text)
            ? 0
            : Convert.ToDouble(MntHTTextBox.Text);

        var detail = new TaxDetail(tauxPercentage, montantHT);

        if (DataContext is FactureViewModel vm && vm.AddTaxDetailCommand.CanExecute(null))
        {
            vm.AddTaxDetailCommand.Execute(detail);
            vm.UpdateStatus();

            TauxTextBox.Text = "";
            MntHTTextBox.Text = "";
        }
        TauxTextBox.Focus();
    }

    public void FocusNumFactureTextBox()
    {
        NumFactureTextBox.Focus();
    }

    private static readonly Regex DecimalRegex = new(@"^\d*\.?\d{0,3}$", RegexOptions.None, TimeSpan.FromMilliseconds(100));
    private static readonly Regex TauxRegex = new(@"^\d{1,3}$", RegexOptions.None, TimeSpan.FromMilliseconds(100));


    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
        e.Handled = !DecimalRegex.IsMatch(fullText);
    }

    private void Taux_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
        e.Handled = !TauxRegex.IsMatch(fullText);
    }
}
