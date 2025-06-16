using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels.Helpers;

public partial class InfoSaisieFacture : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<TaxDetail> ligneFacture;

    [ObservableProperty]
    private string? numCommande;

    [ObservableProperty]
    private string? numSite;

    [ObservableProperty]
    private string? numFacture;

    [ObservableProperty]
    private double? montantTTC;

    [ObservableProperty]
    private double totalTVA = 0f;

    [ObservableProperty]
    private double totalHT = 0f;

    [ObservableProperty]
    private double totalTTC = 0f;

    [ObservableProperty]
    private DateTime dateFacture = DateTime.Now;

    public InfoSaisieFacture()
    {
        LigneFacture = new ObservableCollection<TaxDetail>();
    }

    public void UpdateTotals(TaxDetail taxDetail)
    {
        TotalTVA += taxDetail.MontantTVA;
        TotalHT += taxDetail.MontantHT;
        TotalTTC = TotalTVA + TotalHT;
    }

    public void AddTax(TaxDetail taxDetail)
    {
        LigneFacture.Add(taxDetail);
        UpdateTotals(taxDetail);
    }


    public void RemoveTax(TaxDetail taxDetail)
    {
        if (LigneFacture.Remove(taxDetail))
        {
            RecalculateTotals();
        }
    }

    private void RecalculateTotals()
    {
        TotalHT = LigneFacture.Sum(x => x.MontantHT);
        TotalTVA = LigneFacture.Sum(x => x.MontantTVA);
        TotalTTC = TotalHT + TotalTVA;
    }
}
