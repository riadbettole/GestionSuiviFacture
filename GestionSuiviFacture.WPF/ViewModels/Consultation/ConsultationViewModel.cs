using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.ViewModels.Common;
using GestionSuiviFacture.WPF.ViewModels.Filters;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class ConsultationViewModel : ObservableObject
{
    private readonly IEtiquetteService _etiquetteService;
    private readonly IPrintService _printService;
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

    public ConsultationViewModel(IEtiquetteService etiquetteService, IPrintService printService)
    {
        _etiquetteService = etiquetteService;
        _printService = printService;
        Pagination = new PaginationViewModel();
        Pagination.PageChanged += OnPageChanged;
    }

    private void OnPageChanged(object? sender, int page)
    {
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
                    DateDebut = Filters.DebutDateFilter,
                    DateFin = Filters.FinDateFilter,
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
            _printService.PreviewEtiquette(SelectedEtiquette._etiquette);
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
