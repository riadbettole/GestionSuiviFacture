using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class ConsultationViewModel : ObservableObject
    {
        private readonly ObservableCollection<EtiquetteViewModel> _reservations;
        private readonly EtiquetteService _etiquetteService;

        public ConsultationViewModel()
        {
            _reservations = new ObservableCollection<EtiquetteViewModel>();
            _etiquetteService = new EtiquetteService();
            LoadEtiquettes();
        }

        public IEnumerable<EtiquetteViewModel> Etiquettes => _reservations;

        [RelayCommand]
        private async Task LoadEtiquettes()
        {
            try
            {
                List<Etiquette> etiquettes = [.. await _etiquetteService.Load()];

                UpdateEtiquettes(etiquettes);
            }
            catch (Exception)
            {
            }

        }

        private void UpdateEtiquettes(List<Etiquette> etiquettes)
        {
            _reservations.Clear();

            foreach (Etiquette etiquette in etiquettes)
            {
                _reservations.Add(new EtiquetteViewModel(etiquette));
            }
        }
    }
}
