using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GestionSuiviFacture.WPF.Behaviors;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached(
            "Placeholder",
            typeof(string),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(string.Empty, OnPlaceholderChanged)
        );

    public static string GetPlaceholder(PasswordBox obj) => (string)obj.GetValue(PlaceholderProperty);
    public static void SetPlaceholder(PasswordBox obj, string value) => obj.SetValue(PlaceholderProperty, value);

    private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox passwordBox) return;

        passwordBox.Loaded += (s, _) => UpdatePlaceholder(passwordBox);
        passwordBox.PasswordChanged += (s, _) => UpdatePlaceholder(passwordBox);
    }

    private static void UpdatePlaceholder(PasswordBox passwordBox)
    {
        var placeholder = GetPlaceholder(passwordBox);
        if (string.IsNullOrEmpty(placeholder)) return;

        if (string.IsNullOrEmpty(passwordBox.Password))
        {
            passwordBox.Background = new VisualBrush(new Label
            {
                Content = placeholder,
                Foreground = Brushes.Gray,
                FontFamily = passwordBox.FontFamily,
                FontSize = passwordBox.FontSize,
                Padding = passwordBox.Padding,
                Width = passwordBox.Width,
                Height = passwordBox.Height,
                VerticalContentAlignment = passwordBox.VerticalContentAlignment,
                Background = passwordBox.Background
            })
            {
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Center,
                Stretch = Stretch.None
            };
        }
        else
        {
            passwordBox.Background = Brushes.White;
        }
    }
}