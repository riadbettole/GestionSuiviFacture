using System.Windows;
using System.Windows.Controls;
using GestionSuiviFacture.WPF.ViewModels;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Components.Consultation
{
    /// <summary>
    /// Interaction logic for EtiquettesResultDisplay.xaml
    /// </summary>
    public partial class EtiquettesResultDisplay : UserControl
    {
        public EtiquettesResultDisplay()
        {
            InitializeComponent();
        }


        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var listViewItem = sender as ListViewItem;
                var etiquette = listViewItem?.DataContext as EtiquetteViewModel;

                if (etiquette != null && DataContext is ConsultationViewModel viewModel)
                {
                    viewModel.ShowPopupCommand.Execute(etiquette);
                }
            }
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConsultationViewModel viewModel)
            {
                viewModel.ClosePopupCommand.Execute(null);
            }
        }
    }
}
