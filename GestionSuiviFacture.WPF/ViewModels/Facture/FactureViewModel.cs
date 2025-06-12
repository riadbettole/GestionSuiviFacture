using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.ViewModels.Common;
using GestionSuiviFacture.WPF.ViewModels.Helpers;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class FactureViewModel : ObservableObject
{
    [ObservableProperty]
    private PopupManager<FactureViewModel> alertePopup = new();

    [ObservableProperty]
    private PopupManager<FactureViewModel> notFoundPopup = new();

    [ObservableProperty]
    private InfoSaisieFacture _saisieFacture;

    [ObservableProperty]
    private ObservableCollection<BonDeLivraisonViewModel> bonDeLivraisons;

    [ObservableProperty]
    private CommandeViewModel? _commande;
    private readonly CommandeService _commandeService;

    [ObservableProperty]
    private double _montantTotal = 0;

    [ObservableProperty]
    private string _statut = "AUCUN";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _isSearching = false;

    const string EMPTY = "-----";

    public FactureViewModel()
    {
        _commandeService = new CommandeService();

        SaisieFacture = new InfoSaisieFacture();
        BonDeLivraisons = new ObservableCollection<BonDeLivraisonViewModel>();
        CleanUpCommande();
    }

    private void UpdateMontantTotal()
    {
        double total = 0;

        foreach (BonDeLivraisonViewModel bl in BonDeLivraisons)
        {
            total += bl.montantTTC;
        }

        MontantTotal = total;
    }

    [RelayCommand]
    private async Task FindCommande(String id)
    {
        CleanUpCommande();
        CleanUpSaisie();
        IsSearching = true;
        try
        {
            Commande? commande = await _commandeService.GetCommandeByFilterAsync(
                SaisieFacture.NumSite,
                SaisieFacture.NumCommande
            );
            if (commande == null)
            {
                NoCommandFound();
                return;
            }
            IEnumerable<BonDeLivraison> bonDeLivraison = commande.BonDeLivraison;
            UpdateCommande(commande);
            UpdateBonDeLivraison(bonDeLivraison);
            UpdateMontantTotal();
            CheckAlert();
        }
        finally
        {
            IsSearching = false;
        }
    }

    private void NoCommandFound()
    {
        NotFoundPopup.Show(
            this,
            "COMMANDE OU SITE INTROUVABLE",
            "Ce numero de commande ou site est introuvable. Vérifiez avant de continuer.",
            "#FF5C5C"
        );
    }

    private void UpdateBonDeLivraison(IEnumerable<BonDeLivraison> bonDeLivraison)
    {
        BonDeLivraisons = new ObservableCollection<BonDeLivraisonViewModel>(
            bonDeLivraison.Select(b => new BonDeLivraisonViewModel(b))
        );
    }

    private void UpdateCommande(Commande commande)
    {
        Commande = new CommandeViewModel(commande);
    }

    [RelayCommand]
    private void AddTaxDetail(TaxDetail taxDetail)
    {
        if (taxDetail is null)
            return;

        SaisieFacture.AddTax(taxDetail);
    }


    [RelayCommand]
    private async Task SaveFacture()
    {
        IsLoading = true;

        try
        {
            if (!SaisieFacture.LigneFacture.Any())
            {
                IsLoading = false;
                return;
            }
            EtiquetteDto etiquetteDto = MakeEtiquetteDTO();

            await FactureService.PostEtiquetteAsync(etiquetteDto);

            CleanUpSaisie();
            CleanUpCommande();
            CleanFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private EtiquetteDto MakeEtiquetteDTO()
    {
        return new EtiquetteDto
        {
            NumCommande = SaisieFacture.NumCommande,

            NSite = SaisieFacture.NumSite,
            Site = Commande?.Site,
            LibelleFournisseur = Commande?.NomFournisseur,
            Cnuf = Commande?.CNUF,

            DateCommande = Commande?.DateCommande,
            DateEcheance = Commande?.DateEcheance,
            DateFacture = SaisieFacture.DateFacture,

            Rayon = Commande?.Rayon,
            MontantBRV = Commande?.MontantTTC,
            Groupe = Commande?.Groupe,

            Statut = MapStringIntStatus(Statut),

            NumFacture = SaisieFacture.NumFacture,
            MontantTTCFacture = SaisieFacture.MontantTTC,

            UtilisateurId = AuthService.UserID,

            LigneFactureDTOs = SaisieFacture.LigneFacture.Select(tax => new LigneFactureDto
            {
                MontantHT = tax.MontantHT,
                Taux = tax.TauxPercentage,
            }),
        };
    }

    private static int MapStringIntStatus(string status) =>
        status switch
        {
            "OK" => 0,
            "NOK" => 1,
            "ANNULE" => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(status), "Unknown status value"),
        };

    private void CheckAlert()
    {
        Boolean alertShouldPop = false;
        string title = "FACTURE EN RETARD",
            color = "#FFCF00",
            dates = "",
            message = "La facture a un retard de plus de 6 jours. Vérifiez avant de continuer.";

        var expiredDates = BonDeLivraisons
            .Where(bl => SaisieFacture.DateFacture > bl.dateReception.AddDays(6))
            .Select(bl => bl.dateReception.ToShortDateString());

        if (expiredDates.Any())
        {
            alertShouldPop = true;
            dates = string.Join(", ", expiredDates) + ", ";
        }
        dates = dates.TrimEnd(',', ' ');

        if (SaisieFacture.DateFacture > Commande?.DateEcheance)
        {
            alertShouldPop = true;
            title = "FACTURE ECHUE";
            message = "La facture a passé la date d'échéance. Vérifiez avant de continuer.";
            color = "#FF5C5C";
            dates = Commande.DateEcheance.ToString() ?? "N/A";
        }
        dates += ".";

        if (alertShouldPop)
            ShowPopup(title, message, color, dates);
    }

    private void ShowPopup(string title, string message, string color, string dates)
    {
        AlertePopup.Show(null, title, message, color, dates);
    }

    [RelayCommand]
    private void ClosePopup() => AlertePopup.Close();

    public void ClosePopupAndClean()
    {
        CleanUpCommande();
        AlertePopup.Close();
    }

    private void CleanUpCommande()
    {
        var emptyCommande = new Commande(
            EMPTY, // NomFournisseur
            EMPTY, // CNUF
            EMPTY, // Site
            "---", // Rayon
            EMPTY, // GROUPE
            0, // MontantBRV
            null, // DateCommande
            null, // DateEcheance
            new List<BonDeLivraison>()
        );
        MontantTotal = 0;

        BonDeLivraisons.Clear();
        Commande = new CommandeViewModel(emptyCommande);
    }

    private void CleanUpSaisie()
    {
        SaisieFacture.NumFacture = "";
        SaisieFacture.MontantTTC = null;

        SaisieFacture.LigneFacture.Clear();
        SaisieFacture.TotalHT = 0;
        SaisieFacture.TotalTVA = 0;
        SaisieFacture.TotalTTC = 0;
        Statut = "AUCUN";
    }

    private void CleanFilter()
    {
        SaisieFacture.NumSite = "";
        SaisieFacture.NumCommande = "";
        SaisieFacture.DateFacture = DateTime.Now;
    }

    internal void UpdateStatus()
    {
        Statut = IsItWorking() ? "OK" : "NOK";
    }

    private bool IsItWorking()
    {
        bool isCommandeValid = Math.Abs(MontantTotal - (Commande?.MontantTTC ?? 0.0)) <= 20;
        bool isSaisie1Valid = Math.Abs(MontantTotal - (SaisieFacture?.MontantTTC ?? 0.0)) <= 20;
        bool isSaisie2Valid = Math.Abs(MontantTotal - (SaisieFacture?.TotalTTC ?? 0.0)) <= 20;

        return isCommandeValid && isSaisie1Valid && isSaisie2Valid;
    }

    [RelayCommand]
    public void CloseNotFound() => NotFoundPopup.Close();
}
