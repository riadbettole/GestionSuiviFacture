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
        private readonly EtiquetteService _etiquetteService;
        private readonly ObservableCollection<EtiquetteViewModel> _etiquettes = new();

        [ObservableProperty] private SearchFilters filters = new();
        [ObservableProperty] private PaginationViewModel pagination = new();
        [ObservableProperty] private PopupManager<EtiquetteViewModel> etiquettePopup = new();
        [ObservableProperty] private PopupManager<EtiquetteViewModel> notFoundPopup = new();

        public IEnumerable<EtiquetteViewModel> Etiquettes => _etiquettes;


        public ConsultationViewModel()
        {
            _etiquetteService = new EtiquetteService();

            // Initialize pagination and connect to page change event
            Pagination = new PaginationViewModel();
            Pagination.PageChanged += OnPageChanged;

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


                PaginatedResult<Etiquette> result = await _etiquetteService.GetEtiquettesByFilterAsync(
                    EndOfDay(Filters.DebutDateFilter),
                    EndOfDay(Filters.FinDateFilter),
                    ConvertStatusToEnum(Filters.StatusFilter),
                    Filters.NumSequenceFilter,
                    Filters.NumCommandeFilter,
                    Filters.CnufFilter,
                    Pagination.CurrentPage,                
                    Pagination.PageSize);                  

                UpdateEtiquettes(result.Items);

                if(result.Items.Count() == 1)
                {
                    ShowPopupCommand.Execute(result.Items.First());
                }
                else if(result.Items.Count() == 0)
                {
                    ShowAlert(null);
                }

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
            if (Pagination != null)
                Pagination.PageChanged -= OnPageChanged;

            _etiquettes.Clear();
            if (_etiquetteService is IDisposable d) d.Dispose();
        }

        [RelayCommand] private void ShowPopup(EtiquetteViewModel vm) => EtiquettePopup.Show(vm);
        [RelayCommand] private void ClosePopup() => EtiquettePopup.Close();

        private void ShowAlert(EtiquetteViewModel vm)
        {
            string title = "FACTURE INTROUVABLE";
            string message = "Aucune facture n'a été trouvé. Vérifiez avant de continuer.";
            string color = "#FF5C5C";
            string dates = string.Empty;

            NotFoundPopup.Show(null, title, message, color, dates);
        }
        [RelayCommand] private void CloseNotFoundCommand() => NotFoundPopup.Close();

    }
}