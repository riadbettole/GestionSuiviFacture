using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.ViewModels.Common;
using GestionSuiviFacture.WPF.ViewModels.Filters;
namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class ConsultationViewModel : ObservableObject, IDisposable
    {
        //private readonly IEtiquetteService _etiquetteService;
        private readonly EtiquetteService _etiquetteService;
        private readonly ObservableCollection<EtiquetteViewModel> _etiquettes = new();
        [ObservableProperty] private SearchFilters filters = new();
        [ObservableProperty] private PaginationViewModel pagination = new();
        [ObservableProperty] private PopupManager<EtiquetteViewModel> etiquettePopup = new();
        public IEnumerable<EtiquetteViewModel> Etiquettes => _etiquettes;

        public ConsultationViewModel()
        {
            _etiquetteService = new EtiquetteService();

            // Initialize pagination and connect to page change event
            Pagination = new PaginationViewModel();
            Pagination.PageChanged += OnPageChanged;

            LoadEtiquettesFilterCommand.Execute(null);
        }

        private void OnPageChanged(object sender, int page)
        {
            // When page changes, reload data with the new page
            LoadEtiquettesFilterCommand.Execute(null);
        }

        [RelayCommand]

        private async Task LoadEtiquettesFilterClicked()
        {
            Pagination.CurrentPage = 1;
            await LoadEtiquettesFilter();
        }

        [RelayCommand]
        private async Task LoadEtiquettesFilter()
        {
            try
            {
                //string factureType = Filters.SelectedFactureType switch
                //{
                //    FactureType.Normal => "Normal",
                //    FactureType.Emballage => "Emballage",
                //    _ => null
                //};

                // Pass current page and page size from pagination to your service

                PaginatedResult<Etiquette> result = await _etiquetteService.GetEtiquettesByFilterAsync(
                    EndOfDay(Filters.DebutDateFilter),
                    EndOfDay(Filters.FinDateFilter),
                    ConvertStatusToEnum(Filters.StatusFilter),
                    Filters.NumSequenceFilter,
                    Filters.NumCommandeFilter,
                    Filters.CnufFilter,
                    Pagination.CurrentPage,                // Pass current page
                    Pagination.PageSize);                  // Pass page size

                UpdateEtiquettes(result.Items);
                Pagination.TotalCount = result.TotalCount;
                Pagination.CurrentCount = result.Items.Count();
            }
            catch (Exception) { }
        }

        private void UpdateEtiquettes(IEnumerable<Etiquette> etiquettes)
        {
            _etiquettes.Clear();
            foreach (var etiquette in etiquettes)
                _etiquettes.Add(new EtiquetteViewModel(etiquette));
        }

        private DateTime EndOfDay(DateTime date) => new(date.Year, date.Month, date.Day, 23, 59, 59);

        private StatusEtiquette? ConvertStatusToEnum(string status) => status switch
        {
            "STATUS OK" => StatusEtiquette.OK,
            "STATUS NOK" => StatusEtiquette.NOK,
            "STATUS ANNULE" => StatusEtiquette.ANNULE,
            _ => null
        };

        public void Dispose()
        {
            // Unsubscribe from event when disposing
            if (Pagination != null)
                Pagination.PageChanged -= OnPageChanged;

            _etiquettes.Clear();
            if (_etiquetteService is IDisposable d) d.Dispose();
        }

        [RelayCommand] private void ShowPopup(EtiquetteViewModel vm) => EtiquettePopup.Show(vm);
        [RelayCommand] private void ClosePopup() => EtiquettePopup.Close();
    }
}