using System.Windows;
using System.Windows.Controls;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Views
{
    /// <summary>
    /// Interaction logic for Consultation.xaml
    /// </summary>
    public partial class Consultation : Window
    {
        
        public Consultation()
        {
            InitializeComponent();
            DataContext = new ConsultationViewModel();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
