using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.ViewModels.Common;
using GestionSuiviFacture.WPF.ViewModels.Filters;
using static GestionSuiviFacture.WPF.Services.EtiquetteService;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class ConsultationViewModel : ObservableObject
{
    private readonly EtiquetteService _etiquetteService;
    private readonly ObservableCollection<EtiquetteViewModel> _etiquettes = new();

    [ObservableProperty]
    private SearchFilters filters = new();

    [ObservableProperty]
    private PaginationViewModel pagination = new();

    [ObservableProperty]
    private PopupManager<EtiquetteViewModel> etiquettePopup = new();

    [ObservableProperty]
    private PopupManager<EtiquetteViewModel> notFoundPopup = new();

    [ObservableProperty]
    private bool _isSearching = false;

    public IEnumerable<EtiquetteViewModel> Etiquettes => _etiquettes;

    public ConsultationViewModel()
    {
        _etiquetteService = new EtiquetteService();

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
            var result = await _etiquetteService.GetEtiquettesByFilterAsync(
                new EtiquetteFilterRequest
                {
                    DateDebut = StartOfDay(Filters.DebutDateFilter),
                    DateFin = EndOfDay(Filters.FinDateFilter),
                    Statut = ConvertStatusToEnum(Filters.StatusFilter),
                    N_Sequence = Filters.NumSequenceFilter,
                    N_Commande = Filters.NumCommandeFilter,
                    Cnuf = Filters.CnufFilter,
                    PageNumber = Pagination.CurrentPage,
                    PageSize = Pagination.PageSize,
                }
            );

            UpdateEtiquettes(result.Items);

            Pagination.TotalCount = result.TotalCount;
            Pagination.CurrentCount = result.Items.Count();

            if (result.Items.Any() && Pagination.TotalCount == 1)
            {
                SelectedEtiquette = new EtiquetteViewModel(result.Items.First());
                ShowPopupCommand.Execute(null);
            }
            else if (!result.Items.Any())
            {
                string title = "FACTURE INTROUVABLE";
                string message = "Aucune facture n'a été trouvé. Vérifiez avant de continuer.";
                string color = "#FF5C5C";
                ShowAlert(title, message, color);
            }
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode.GetValueOrDefault() == HttpStatusCode.NotFound)
            {
                ShowAlert("ERREUR RESSAYER", "ERREUR RESSAYER", "#FF5C5C");
            }
        }
        catch (Exception)
        {
            ShowAlert(
                "PAS DE CONNEXION",
                "Aucune connexion n'a été établie. Vérifiez avant de continuer.",
                "#FF5C5C"
            );
        }
    }

    private void UpdateEtiquettes(IEnumerable<Etiquette> etiquettes)
    {
        _etiquettes.Clear();
        foreach (var etiquette in etiquettes)
            _etiquettes.Add(new EtiquetteViewModel(etiquette));
    }

    private static DateTime StartOfDay(DateTime date) =>
        new(date.Year, date.Month, date.Day, 00, 00, 00, DateTimeKind.Local);

    private static DateTime EndOfDay(DateTime date)
    {
        DateTime newDate = date.AddDays(1);
        return new DateTime(
            newDate.Year,
            newDate.Month,
            newDate.Day,
            00,
            00,
            01,
            DateTimeKind.Local
        );
    }

    private static int? ConvertStatusToEnum(string status) =>
        status switch
        {
            "STATUS OK" => 0,
            "STATUS NOK" => 1,
            "STATUS ANNULÉ" => 2,
            _ => null,
        };

    [ObservableProperty]
    private EtiquetteViewModel? selectedEtiquette;

    [RelayCommand]
    private void PreviewEtiquette(object parameter)
    {
        if (SelectedEtiquette != null)
            PrintService.ShowPrintPreview(SelectedEtiquette._etiquette);
    }

    [RelayCommand]
    private void ShowPopup()
    {
        if (SelectedEtiquette != null)
            EtiquettePopup.Show(SelectedEtiquette);
    }

    [RelayCommand]
    private void ClosePopup() => EtiquettePopup.Close();

    private void ShowAlert(string title, string message, string color)
    {
        NotFoundPopup.Show(null, title, message, color, string.Empty);
    }

    [RelayCommand]
    private void CloseNotFound() => NotFoundPopup.Close();
}
