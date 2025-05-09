using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class ConsultationViewModel : ObservableObject, IDisposable
    {
        private readonly ObservableCollection<EtiquetteViewModel> _etiquettes;
        private readonly EtiquetteService _etiquetteService;

        [ObservableProperty]
        private string _numSequenceFilter;

        [ObservableProperty]
        private string _fournisseurFilter;

        [ObservableProperty]
        private string _cnufFilter;

        [ObservableProperty]
        private string _numCommandeFilter;

        [ObservableProperty]
        private DateTime _debutDateFilter = DateTime.Now;

        [ObservableProperty]
        private DateTime _finDateFilter = DateTime.Now;

        [ObservableProperty]
        private string _statusFilter = "TOUT";

        [ObservableProperty]
        private string _factureTypeFilter;

        public List<string> StatusOptions { get; } =
            new() { "STATUS OK", "STATUS NOK", "STATUS ANNULE", "TOUT" };

        public ConsultationViewModel()
        {
            _etiquettes = new ObservableCollection<EtiquetteViewModel>();
            _etiquetteService = new EtiquetteService();
            LoadEtiquettesFilter();
        }

        public IEnumerable<EtiquetteViewModel> Etiquettes => _etiquettes;


        [RelayCommand]
        private async Task LoadEtiquettesFilter()
        {
            try
            {
                IEnumerable<Etiquette> etiquettes = await _etiquetteService.GetAllEtiquette();
                
                //IEnumerable<Etiquette> etiquettes = await _etiquetteService.GetEtiquetteByFilter(
                //    NumSequenceFilter,
                //    DebutDateFilter,
                //    EndOfDay(FinDateFilter),
                //    NumCommandeFilter,
                //    CnufFilter,
                //    ConvertStatusToEnum(StatusFilter)
                //);

                UpdateEtiquettes(etiquettes);
            }
            catch (Exception) { }
        }
        private void UpdateEtiquettes(IEnumerable<Etiquette> etiquettes)
        {
            _etiquettes.Clear();

            foreach (Etiquette etiquette in etiquettes)
            {
                _etiquettes.Add(new EtiquetteViewModel(etiquette));
            }
        }

        private DateTime EndOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }

        private StatusEtiquette? ConvertStatusToEnum(string statusFilter)
        {
            return statusFilter switch
            {
                "STATUS OK" => StatusEtiquette.OK,
                "STATUS NOK" => StatusEtiquette.NOK,
                "STATUS ANNULE" => StatusEtiquette.ANNULE,
                _ => null, // null for "TOUT"
            };
        }

        public void Dispose()
        {
            _etiquettes.Clear();

            // Dispose of service if needed
            if (_etiquetteService is IDisposable disposableService)
            {
                disposableService.Dispose();
            }

            // Cancel any pending tasks if you have any
            // If you have a CancellationTokenSource, dispose it here

            // Suggest garbage collection
            GC.Collect();
        }



      
        [ObservableProperty]
        private EtiquetteViewModel _selectedEtiquette;

        [ObservableProperty]
        private bool _isPopupVisible;


        [RelayCommand]
        private void ShowPopup(EtiquetteViewModel etiquette)
        {
            SelectedEtiquette = etiquette;
            IsPopupVisible = true;
        }

        [RelayCommand]
        private void ClosePopup()
        {
            IsPopupVisible = false;
        }
    }
}
