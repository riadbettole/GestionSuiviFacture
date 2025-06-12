using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Components;

public partial class PageLayout : UserControl
{
    public PageLayout()
    {
        InitializeComponent();
    }

    private void DragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Window window = Window.GetWindow(this);
        window?.DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this).WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // You might want to add a confirmation dialog here
        Window.GetWindow(this).Close();
    }
}
