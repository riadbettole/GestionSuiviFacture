using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionSuiviFacture.WPF.DTOs;
using GestionSuiviFacture.WPF.Models;
using GestionSuiviFacture.WPF.Services;
using GestionSuiviFacture.WPF.Services.Saisie;
using GestionSuiviFacture.WPF.ViewModels.Common;
using GestionSuiviFacture.WPF.ViewModels.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.Windows;

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

    [ObservableProperty]
    private double _montantTotal = 0;

    [ObservableProperty]
    private string _statut = "AUCUN";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _isSearching = false;

    const string EMPTY = "-----";

    private readonly ICommandeService _commandeService;
    private readonly IFactureService _factureService;
    private readonly IAuthService _authService;
    private readonly IPrintService _printService;

    public FactureViewModel(IAuthService authService, ICommandeService commandeService, IFactureService factureService, IPrintService printService)
    {
        _commandeService = commandeService;
        _factureService = factureService;
        _authService = authService;
        _printService = printService;

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
    private void RemoveTaxDetail(TaxDetail taxDetail)
    {
        if (taxDetail != null)
        {
            SaisieFacture.RemoveTax(taxDetail);
            UpdateStatus();
        }
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
                return; // ← PROBLÈME ICI : on sort sans sauvegarder
            }

            EtiquetteDto etiquetteDto = MakeEtiquetteDTO();

            // ← VÉRIFIEZ QUE CETTE LIGNE S'EXÉCUTE
            var result = await _factureService.PostEtiquetteAsync(etiquetteDto);

            // ← ET QUE CETTE LIGNE AUSSI
            Etiquette etq = MakeEtiquette(result.NSequence, result.DateTraitement, _authService.Username);

            // Prévisualisation (ne devrait pas bloquer la sauvegarde)
            _printService.PreviewEtiquette(etq);

            CleanUpSaisie();
            CleanUpCommande();
            CleanFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }
    private Etiquette MakeEtiquette(string seq, DateTime? date, string user)
    {
        return new Etiquette(
            date,
            seq,
            MapStringIntStatus(Statut),
            Commande?.DateCommande,
            Commande?.MontantTTC,
            SaisieFacture?.NumCommande,
            Commande?.NomFournisseur,
            Commande?.Site,
            Commande?.Groupe,
            "",
            Commande?.CNUF,
            SaisieFacture?.DateFacture,
            SaisieFacture?.NumFacture,
            SaisieFacture?.MontantTTC,
            null, null, null, null, null,
            user
        );
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

            UtilisateurId = _authService.UserID,

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
