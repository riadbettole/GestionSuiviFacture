using System.Windows;
using System.Windows.Controls;
using GestionSuiviFacture.WPF.ViewModels;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Views
{
    /// <summary>
    /// Interaction logic for FactureEmballage.xaml
    /// </summary>
    public partial class FactureEmballage : UserControl
    {
        public FactureEmballage()
        {
            InitializeComponent();
            SearchDisplayControl.EnterPressed += SearchDisplayControl_EnterPressed;
            AlertDisplayControl.ButtonPressed += SearchDisplayControl_ButtonPressed;
        }


        private void SearchDisplayControl_EnterPressed(object sender, EventArgs e)
        {
            SaisisInfoFactureDisplay.FocusNumFactureTextBox();
        }


        private void SearchDisplayControl_ButtonPressed(object sender, EventArgs e)
        {
            SearchDisplayControl.CleanAll();
        }

    }
}
