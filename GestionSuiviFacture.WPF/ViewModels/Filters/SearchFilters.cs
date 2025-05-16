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
        private IEnumerable<String> statusOptions =
        [
            "STATUS OK",
            "STATUS NOK",
            "STATUS ANNULE",
            "TOUT",
        ];
        [ObservableProperty]
        private FactureType selectedFactureType = FactureType.None;

        //[ObservableProperty]
        public bool IsNormalFactureType => SelectedFactureType == FactureType.Normal;
        public bool IsEmballageFactureType => SelectedFactureType == FactureType.Emballage;

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
    }
}