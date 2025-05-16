using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace GestionSuiviFacture.WPF.ViewModels.Common;

public partial class PaginationViewModel : ObservableObject
{
    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int pageSize = 2;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private int currentCount;

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    // Calculate the starting position for the current page
    public int StartPosition => (CurrentPage - 1) * PageSize + 1;

    // Calculate the ending position for the current page
    public int EndPosition => Math.Min(CurrentPage * PageSize, TotalCount);

    public event EventHandler<int> PageChanged;

    [RelayCommand]
    private void GoToFirstPage()
    {
        if (HasPreviousPage)
        {
            CurrentPage = 1;
            PageChanged?.Invoke(this, CurrentPage);
        }
    }

    [RelayCommand]
    private void GoToPreviousPage()
    {
        if (HasPreviousPage)
        {
            CurrentPage--;
            PageChanged?.Invoke(this, CurrentPage);
        }
    }

    [RelayCommand]
    private void GoToNextPage()
    {
        if (HasNextPage)
        {
            CurrentPage++;
            PageChanged?.Invoke(this, CurrentPage);
        }
    }

    [RelayCommand]
    private void GoToLastPage()
    {
        if (HasNextPage)
        {
            CurrentPage = TotalPages;
            PageChanged?.Invoke(this, CurrentPage);
        }
    }

    partial void OnCurrentPageChanged(int value)
    {
        OnPropertyChanged(nameof(HasPreviousPage));
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(StartPosition));
        OnPropertyChanged(nameof(EndPosition));
    }

    partial void OnTotalCountChanged(int value)
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(EndPosition));
    }

    partial void OnPageSizeChanged(int value)
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(StartPosition));
        OnPropertyChanged(nameof(EndPosition));
    }
}