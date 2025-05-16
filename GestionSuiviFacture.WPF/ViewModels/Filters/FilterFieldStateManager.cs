using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class FilterFieldStateManager : ObservableObject
    {
        [ObservableProperty] private bool isNumSequenceEnabled = true;
        [ObservableProperty] private bool isNumCommandeEnabled = true;
        [ObservableProperty] private bool isCnufEnabled = true;

        public void UpdateStates(string numSeq, string numCmd, string cnuf)
        {
            bool hasNumSequence = !string.IsNullOrWhiteSpace(numSeq);
            bool hasNumCommande = !string.IsNullOrWhiteSpace(numCmd);
            bool hasCnuf = !string.IsNullOrWhiteSpace(cnuf);

            IsNumSequenceEnabled = !hasNumCommande && !hasCnuf;
            IsNumCommandeEnabled = !hasNumSequence && !hasCnuf;
            IsCnufEnabled = !hasNumSequence && !hasNumCommande;
        }
    }
}
