using System.Windows.Controls;

namespace GestionSuiviFacture.WPF.Views;

public partial class FactureEmballage : UserControl
{
    public FactureEmballage()
    {
        InitializeComponent();
        SearchDisplayControl.EnterPressed += SearchDisplayControl_EnterPressed;
        AlertDisplayControl.ButtonPressed += SearchDisplayControl_ButtonPressed;
    }

    private void SearchDisplayControl_EnterPressed(object? sender, EventArgs e)
    {
        SaisisInfoFactureDisplay.FocusNumFactureTextBox();
    }

    private void SearchDisplayControl_ButtonPressed(object? sender, EventArgs e)
    {
        SearchDisplayControl.CleanAll();
    }
}
