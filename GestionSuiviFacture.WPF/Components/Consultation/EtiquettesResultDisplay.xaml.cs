using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestionSuiviFacture.WPF.ViewModels;

namespace GestionSuiviFacture.WPF.Components.Consultation;

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
                viewModel.SelectedEtiquette = etiquette;
                viewModel.ShowPopupCommand.Execute(null);
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
