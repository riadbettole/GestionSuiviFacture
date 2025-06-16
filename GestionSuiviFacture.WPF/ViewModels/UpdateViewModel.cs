using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionSuiviFacture.WPF.ViewModels;

public partial class UpdateViewModel : ObservableObject
{
    [ObservableProperty]
    private string status = "Initialisation...";

    [ObservableProperty]
    private bool isIndeterminate = true;

    [ObservableProperty]
    private double progressValue = 0;

    [ObservableProperty]
    private string progressText = "";

    public event Action? UpdateCheckCompleted;

    public void AssignAction(Action action)
    {
        UpdateCheckCompleted += action;
    }

    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
    }

    public void SetProgress(double value, bool indeterminate = false, string progressText = "")
    {
        ProgressValue = value;
        IsIndeterminate = indeterminate;
        ProgressText = progressText;
    }

    public void CompleteUpdateCheck()
    {
        UpdateCheckCompleted?.Invoke();
    }
}
