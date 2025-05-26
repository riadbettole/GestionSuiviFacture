using System.Windows;
using System.Windows.Controls;
using GestionSuiviFacture.WPF.ViewModels;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace GestionSuiviFacture.WPF.Components.Facture
{
    /// <summary>
    /// Interaction logic for SearchSaisieDisplay.xaml
    /// </summary>
    public partial class SearchSaisieDisplay : UserControl
    {

        public event EventHandler EnterPressed;


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
            if (e.Key == Key.Enter)
            {
                if (DataContext is FactureViewModel vm && vm.FindCommandeCommand.CanExecute(null))
                {
                    vm.FindCommandeCommand.Execute(null);
                    EnterPressed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
            
        public void CleanAll()
        {
            DateTextBox.SelectedDate = DateTime.Now;
            SiteTextBox.Clear();
            CommandeTextBox.Clear();

            DateTextBox.Focus();
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
