using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels.Filters;

public partial class SearchFilters : ObservableObject
{
    [ObservableProperty]
    private string? numSequenceFilter;

    [ObservableProperty]
    private string? numCommandeFilter;

    [ObservableProperty]
    private string? cnufFilter;

    [ObservableProperty]
    private string? fournisseurFilter;

    [ObservableProperty]
    private DateTime debutDateFilter = DateTime.Now;

    [ObservableProperty]
    private DateTime finDateFilter = DateTime.Now;

    [ObservableProperty]
    private string statusFilter = "TOUT";

    [ObservableProperty]
    private bool _isNormalFactureType = true;

    [ObservableProperty]
    private bool _isEmballageFactureType;

    [ObservableProperty]
    private IEnumerable<string> statusOptions =
    [
        "STATUS OK",
        "STATUS NOK",
        "STATUS ANNULÉ",
        "TOUT",
    ];

    [ObservableProperty]
    private FactureType selectedFactureType = FactureType.None;

    [ObservableProperty]
    private bool isNumSequenceEnabled = true;

    [ObservableProperty]
    private bool isNumCommandeEnabled = true;

    [ObservableProperty]
    private bool isCnufEnabled = true;

    public FilterFieldStateManager FieldStates { get; } = new();

    partial void OnNumSequenceFilterChanged(string? value)
    {
        UpdateEnabledStates();
        UpdateTextBoxStates();
    }

    partial void OnNumCommandeFilterChanged(string? value)
    {
        UpdateEnabledStates();
        UpdateTextBoxStates();
    }

    partial void OnCnufFilterChanged(string? value)
    {
        UpdateEnabledStates();
        UpdateTextBoxStates();
    }

    private void UpdateEnabledStates()
    {
        bool hasNumSequence = !string.IsNullOrWhiteSpace(NumSequenceFilter);
        bool hasNumCommande = !string.IsNullOrWhiteSpace(NumCommandeFilter);
        bool hasCnuf = !string.IsNullOrWhiteSpace(CnufFilter);

        IsNumSequenceEnabled = !hasNumCommande && !hasCnuf;
        IsNumCommandeEnabled = !hasNumSequence && !hasCnuf;
        IsCnufEnabled = !hasNumSequence && !hasNumCommande;
    }

    partial void OnDebutDateFilterChanged(DateTime value)
    {
        if (FinDateFilter < value)
            FinDateFilter = value;
    }

    partial void OnFinDateFilterChanged(DateTime value)
    {
        if (DebutDateFilter > value)
            DebutDateFilter = value;
    }

    private void UpdateTextBoxStates()
    {
        FieldStates.UpdateStates(
            NumSequenceFilter ?? "",
            NumCommandeFilter ?? "",
            CnufFilter ?? ""
        );
    }

    partial void OnIsNormalFactureTypeChanged(bool value)
    {
        if (value)
            IsEmballageFactureType = false;
    }

    partial void OnIsEmballageFactureTypeChanged(bool value)
    {
        if (value)
            IsNormalFactureType = false;
    }

    [ObservableProperty]
    private string _searchButtonText = "Rechercher";
}
