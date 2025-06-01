using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace GestionSuiviFacture.WPF.ViewModels.Filters
{
    public partial class SearchFilters : ObservableObject
    {
        [ObservableProperty] private string numSequenceFilter;
        [ObservableProperty] private string numCommandeFilter;
        [ObservableProperty] private string cnufFilter;
        [ObservableProperty] private string fournisseurFilter;
        [ObservableProperty] private DateTime debutDateFilter = DateTime.Now;
        [ObservableProperty] private DateTime finDateFilter = DateTime.Now;
        [ObservableProperty] private string statusFilter = "TOUT";

        [ObservableProperty]
        private bool _isNormalFactureType = true;

        [ObservableProperty]
        private bool _isEmballageFactureType;

        [ObservableProperty]
        private IEnumerable<String> statusOptions =
        [
            "STATUS OK",
            "STATUS NOK",
            "STATUS ANNULÉ",
            "TOUT",
        ];
        [ObservableProperty]
        private FactureType selectedFactureType = FactureType.None;

        public FilterFieldStateManager FieldStates { get; } = new();

        partial void OnNumSequenceFilterChanged(string value) => UpdateTextBoxStates();
        partial void OnNumCommandeFilterChanged(string value) => UpdateTextBoxStates();
        partial void OnCnufFilterChanged(string value) => UpdateTextBoxStates();

        partial void OnDebutDateFilterChanged(DateTime value)
        {
            if (FinDateFilter < value) FinDateFilter = value;
        }

        partial void OnFinDateFilterChanged(DateTime value)
        {
            if (DebutDateFilter > value) DebutDateFilter = value;
        }

        private void UpdateTextBoxStates()
        {
            FieldStates.UpdateStates(NumSequenceFilter, NumCommandeFilter, CnufFilter);
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
    }
}