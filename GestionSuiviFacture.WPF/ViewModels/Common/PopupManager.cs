using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class PopupManager<T> : ObservableObject
    {
        [ObservableProperty] public T? selectedItem;
        [ObservableProperty] public bool isVisible = false;

        [ObservableProperty] public string title = "ALERTE EXEMPLE";
        [ObservableProperty] public string message = "La facture a un example de plus de example jours. Vérifiez avant de exemple.";
        [ObservableProperty] public string color = "#FFCF00";
        [ObservableProperty] public string dates = "00/00/0001, 01/00/0001, 02/00/0001";

        public void Show(T? item, string title = "", string message = "", string color = "", string dates = "")
        {
            SelectedItem = item;

            Title = title;
            Message = message;
            Color = color;
            Dates = dates;

            IsVisible = true;
        }

        public void Close() => IsVisible = false;
    }
}
