using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels.Common;

public partial class PopupManager<T> : ObservableObject
{
    [ObservableProperty]
    private T? selectedItem;

    [ObservableProperty]
    private bool isVisible = false;

    [ObservableProperty]
    private string title = "ALERTE EXEMPLE";

    [ObservableProperty]
    private string message =
        "La facture a un example de plus de example jours. Vérifiez avant de exemple.";

    [ObservableProperty]
    private string color = "#FFCF00";

    [ObservableProperty]
    private string dates = "00/00/0001, 01/00/0001, 02/00/0001";

    public void Show(
        T? item,
        string title = "",
        string message = "",
        string color = "",
        string dates = ""
    )
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
