using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Components.Facture;

public partial class SearchSaisieDisplay : UserControl
{
    public event EventHandler? EnterPressed;

    public SearchSaisieDisplay()
    {
        InitializeComponent();
    }

    private void DateTextBox_Loaded(object sender, RoutedEventArgs e)
    {
        DateTextBox.Focus();
    }

    private void NumCommandeInputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (
            e.Key == Key.Enter
            && DataContext is FactureViewModel vm
            && vm.FindCommandeCommand.CanExecute(null)
        )
        {
            vm.FindCommandeCommand.Execute(null);
            EnterPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DateInputBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SiteTextBox.Focus();
            e.Handled = true;
        }
    }

    public void CleanAll()
    {
        DateTextBox.SelectedDate = DateTime.Now;
        SiteTextBox.Clear();
        CommandeTextBox.Clear();

        DateTextBox.Focus();
    }

    private void SiteInputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CommandeTextBox.Focus();
        }
    }
}
