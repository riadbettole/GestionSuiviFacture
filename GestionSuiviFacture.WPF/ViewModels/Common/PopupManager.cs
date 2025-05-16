using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels
{
    public partial class PopupManager<T> : ObservableObject
    {
        [ObservableProperty] public T selectedItem;
        [ObservableProperty] public bool isVisible;

        public void Show(T item)
        {
            SelectedItem = item;
            IsVisible = true;
        }

        public void Close() => IsVisible = false;
    }
}
