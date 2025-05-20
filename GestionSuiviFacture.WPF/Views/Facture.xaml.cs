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
        }


        private void SearchDisplayControl_EnterPressed(object sender, EventArgs e)
        {
            SaisisInfoFactureDisplay.FocusNumFactureTextBox();
        }



    }
}