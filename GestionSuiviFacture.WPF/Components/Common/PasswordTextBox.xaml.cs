using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Components.Common;

/// <summary>
/// Logique d'interaction pour PasswordTextBox.xaml
/// </summary>
public partial class PasswordTextBox : UserControl
{
    private bool isTextVisible = false;

    public PasswordTextBox()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            ToggleTextVisibility();
        }
    }

    private void ToggleTextVisibility()
    {
        isTextVisible = !isTextVisible;
        UpdateTextVisibility();
    }

    private void UpdateTextVisibility()
    {
        if (isTextVisible)
        {
            textBox.Text = SecureText;
        }
        else
        {
            SecureText = textBox.Text;
            textBox.Text = new string('*', SecureText.Length);
        }
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        isTextVisible = true;
        UpdateTextVisibility();
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        isTextVisible = false;
        UpdateTextVisibility();
    }

    public static readonly DependencyProperty SecureTextProperty =
        DependencyProperty.Register("SecureText", typeof(string), typeof(PasswordTextBox));

    public string SecureText
    {
        get { return (string)GetValue(SecureTextProperty); }
        set { SetValue(SecureTextProperty, value); }
    }
}
