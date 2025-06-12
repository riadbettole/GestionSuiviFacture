using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GestionSuiviFacture.WPF.Behaviors;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.RegisterAttached(
            "PlaceholderText",
            typeof(string),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(string.Empty)
        );

    public static readonly DependencyProperty IsAttachedProperty =
        DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(false, OnIsAttachedChanged)
        );

    public static string GetPlaceholderText(PasswordBox passwordBox)
    {
        return (string)passwordBox.GetValue(PlaceholderTextProperty);
    }

    public static void SetPlaceholderText(PasswordBox passwordBox, string value)
    {
        passwordBox.SetValue(PlaceholderTextProperty, value);
    }

    public static bool GetIsAttached(PasswordBox passwordBox)
    {
        return (bool)passwordBox.GetValue(IsAttachedProperty);
    }

    public static void SetIsAttached(PasswordBox passwordBox, bool value)
    {
        passwordBox.SetValue(IsAttachedProperty, value);
    }

    private static void OnIsAttachedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
    )
    {
        if (d is not PasswordBox passwordBox)
            return;

        if ((bool)e.NewValue)
            AttachPlaceholder(passwordBox);
        else
            DetachPlaceholder(passwordBox);
    }

    private static void AttachPlaceholder(PasswordBox passwordBox)
    {
        passwordBox.Loaded += (s, e) =>
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(passwordBox);
            if (adornerLayer == null)
                return;

            var placeholderAdorner = new PasswordBoxPlaceholderAdorner(passwordBox);
            adornerLayer.Add(placeholderAdorner);

            passwordBox.PasswordChanged += OnPasswordChanged;
            UpdatePlaceholderVisibility(passwordBox);
        };
    }

    private static void DetachPlaceholder(PasswordBox passwordBox)
    {
        passwordBox.PasswordChanged -= OnPasswordChanged;

        var adornerLayer = AdornerLayer.GetAdornerLayer(passwordBox);
        if (adornerLayer == null)
            return;

        var adorners = adornerLayer.GetAdorners(passwordBox);
        if (adorners == null)
            return;

        adorners.Where(adorner => adorner is PasswordBoxPlaceholderAdorner)
        .ToList()
        .ForEach(adorner => adornerLayer.Remove(adorner));
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
            UpdatePlaceholderVisibility(passwordBox);
    }

    private static void UpdatePlaceholderVisibility(PasswordBox passwordBox)
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(passwordBox);
        if (adornerLayer == null)
            return;

        var adorners = adornerLayer.GetAdorners(passwordBox);
        if (adorners == null)
            return;

        foreach (var adorner in adorners)
        {
            if (adorner is PasswordBoxPlaceholderAdorner placeholderAdorner)
            {
                placeholderAdorner.IsPlaceholderVisible = string.IsNullOrEmpty(
                    passwordBox.Password
                );
                adornerLayer.Update(passwordBox);
            }
        }
    }

    private sealed class PasswordBoxPlaceholderAdorner : Adorner
    {
        private readonly PasswordBox _passwordBox;
        private bool _isPlaceholderVisible = true;

        public bool IsPlaceholderVisible
        {
            get => _isPlaceholderVisible;
            set
            {
                if (_isPlaceholderVisible != value)
                {
                    _isPlaceholderVisible = value;
                    InvalidateVisual();
                }
            }
        }

        public PasswordBoxPlaceholderAdorner(PasswordBox passwordBox)
            : base(passwordBox)
        {
            _passwordBox = passwordBox;
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!IsPlaceholderVisible)
                return;

            var placeholderText = GetPlaceholderText(_passwordBox);
            if (string.IsNullOrEmpty(placeholderText))
                return;

            var padding = _passwordBox.Padding;
            var formattedText = new FormattedText(
                placeholderText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    _passwordBox.FontFamily,
                    _passwordBox.FontStyle,
                    _passwordBox.FontWeight,
                    _passwordBox.FontStretch
                ),
                _passwordBox.FontSize,
                Brushes.Gray,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            var textPosition = new Point(
                padding.Left,
                (_passwordBox.ActualHeight - formattedText.Height) / 2
            );

            drawingContext.DrawText(formattedText, textPosition);
        }
    }
}
