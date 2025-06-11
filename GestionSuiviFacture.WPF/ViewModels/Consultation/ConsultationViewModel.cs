using System.Collections.ObjectModel;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services.Utilities;
using GestionSuiviFacture.WPF.ViewModels.Common;
using GestionSuiviFacture.WPF.ViewModels.Filters;
namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class ConsultationViewModel : ObservableObject, IDisposable
    {
        private readonly PrintService _printService;
        private readonly EtiquetteService _etiquetteService;
        private readonly ObservableCollection<EtiquetteViewModel> _etiquettes = new();

        [ObservableProperty] private SearchFilters filters = new();
        [ObservableProperty] private PaginationViewModel pagination = new();
        [ObservableProperty] private PopupManager<EtiquetteViewModel> etiquettePopup = new();
        [ObservableProperty] private PopupManager<EtiquetteViewModel> notFoundPopup = new();

        [ObservableProperty]
        private bool _isSearching = false;

        public IEnumerable<EtiquetteViewModel> Etiquettes => _etiquettes;


        public ConsultationViewModel()
        {
            _etiquetteService = new EtiquetteService();
            _printService = new PrintService();

            // Initialize pagination and connect to page change event
            Pagination = new PaginationViewModel();
            Pagination.PageChanged += OnPageChanged;

        }

        private void OnPageChanged(object? sender, int page)
        {
            // When page changes, reload data with the new page
            LoadEtiquettesFilterCommand.Execute(null);
        }

        [RelayCommand]

        private async Task LoadEtiquettesFilterClicked()
        {
            IsSearching = true;
            try
            {
                Pagination.CurrentPage = 1;
                await LoadEtiquettesFilter();
            }
            finally
            {
                IsSearching = false;
            }
        }

        [RelayCommand]
        private async Task LoadEtiquettesFilter()
        {
            try
            {
                
                PaginatedResult<Etiquette> result = new();
           
                result = await _etiquetteService.GetEtiquettesByFilterAsync(
                    StartOfDay(Filters.DebutDateFilter),
                    EndOfDay(Filters.FinDateFilter),
                    ConvertStatusToEnum(Filters.StatusFilter),
                    Filters.NumSequenceFilter,
                    Filters.NumCommandeFilter,
                    Filters.CnufFilter,
                    Pagination.CurrentPage,                
                    Pagination.PageSize);

                UpdateEtiquettes(result.Items);


                Pagination.TotalCount = result.TotalCount;
                Pagination.CurrentCount = result.Items.Count();

                if (result.Items.Count() == 1 && Pagination.TotalCount == 1)
                {
                    SelectedEtiquette = new EtiquetteViewModel(result.Items.First());
                    ShowPopupCommand.Execute(null);
                }
                else if(result.Items.Count() == 0)
                {
                    string title = "FACTURE INTROUVABLE";
                    string message = "Aucune facture n'a été trouvé. Vérifiez avant de continuer.";
                    string color = "#FF5C5C";
                    ShowAlert(title, message, color);
                }

                
            }
            catch(HttpRequestException exception)
            {
                if ((int)exception.StatusCode == 404)
                {
                    ShowAlert(
                     "ERREUR RESSAYER",
                     "ERREUR RESSAYER",
                     "#FF5C5C");
                }
            }
            catch (Exception)
            {
                ShowAlert(
                      "PAS DE CONNEXION",
                      "Aucune connexion n'a été établie. Vérifiez avant de continuer.",
                      "#FF5C5C");
            }
        }

        private void UpdateEtiquettes(IEnumerable<Etiquette> etiquettes)
        {
            _etiquettes.Clear();
            foreach (var etiquette in etiquettes)
                _etiquettes.Add(new EtiquetteViewModel(etiquette));
        }

        private DateTime StartOfDay(DateTime date) => new(date.Year, date.Month, date.Day, 00, 00, 00);
        private DateTime EndOfDay(DateTime date)
        {
            DateTime newDate = date.AddDays(1);
            return new(newDate.Year, newDate.Month, newDate.Day, 00, 00, 01);
        }

        private int? ConvertStatusToEnum(string status) => status switch
        {
            "STATUS OK" => 0,
            "STATUS NOK" => 1,
            "STATUS ANNULE" => 2,
            _ => null
        };

        public void Dispose()
        {
            if (Pagination != null)
                Pagination.PageChanged -= OnPageChanged;

            _etiquettes.Clear();
            if (_etiquetteService is IDisposable d) d.Dispose();
        }



        [ObservableProperty]
        public EtiquetteViewModel? selectedEtiquette;

        [RelayCommand]
        private void PreviewEtiquette(object parameter)
        {
            if (SelectedEtiquette != null)
                _printService.ShowPrintPreview(SelectedEtiquette._etiquette);
        }


        [RelayCommand]
        private void ShowPopup()
        {
            if (SelectedEtiquette != null)
                EtiquettePopup.Show(SelectedEtiquette);
        }

        [RelayCommand] private void ClosePopup() => EtiquettePopup.Close();

        private void ShowAlert(string title, string message, string color)
        {
            NotFoundPopup.Show(null, title, message, color, string.Empty);
        }
        [RelayCommand] private void CloseNotFound() => NotFoundPopup.Close();

    }
}