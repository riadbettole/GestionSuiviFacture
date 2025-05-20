using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class PopupManager<T> : ObservableObject
    {
        [ObservableProperty] public T selectedItem;
        [ObservableProperty] public bool isVisible = false;

        [ObservableProperty] public string title = "ALERTE EXEMPLE";
        [ObservableProperty] public string message = "La facture a un example de plus de example jours. Vérifiez avant de exemple.";

        public void Show(T item)
        {
            SelectedItem = item;
            IsVisible = true;
        }

        public void Close() => IsVisible = false;
    }
}
