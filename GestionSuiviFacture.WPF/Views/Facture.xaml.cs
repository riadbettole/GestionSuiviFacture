using GestionSuiviFacture.WPF.Components.Common;
using System.Windows.Controls;

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