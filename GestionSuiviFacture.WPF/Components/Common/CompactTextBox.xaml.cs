using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GestionSuiviFacture.WPF.Components.Common;

public partial class CompactTextBox : UserControl, INotifyPropertyChanged
{
    public CompactTextBox()
    {
        InitializeComponent();
        textBox.GotFocus += TextBox_GotFocus;
        textBox.LostFocus += TextBox_LostFocus;
        textBox.TextChanged += TextBox_TextChanged;
        Loaded += CompactTextBox_Loaded;
    }

    #region Dependency Properties

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text",
        typeof(string),
        typeof(CompactTextBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
        )
    );

    public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        "PlaceholderText",
        typeof(string),
        typeof(CompactTextBox),
        new PropertyMetadata("Enter text...")
    );

    public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
        "IconSource",
        typeof(string),
        typeof(CompactTextBox),
        new PropertyMetadata("/Images/Search.png")
    );

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public string PlaceholderText
    {
        get { return (string)GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    public string IconSource
    {
        get { return (string)GetValue(IconSourceProperty); }
        set { SetValue(IconSourceProperty, value); }
    }

    #endregion

    #region Events

    public event RoutedEventHandler? ButtonClick;
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Properties

    private bool _isPlaceholderVisible = true;
    public bool IsPlaceholderVisible
    {
        get { return _isPlaceholderVisible; }
        set
        {
            _isPlaceholderVisible = value;
            OnPropertyChanged(nameof(IsPlaceholderVisible));
        }
    }

    #endregion

    #region Event Handlers

    private void CompactTextBox_Loaded(object sender, RoutedEventArgs e)
    {
        UpdatePlaceholderVisibility();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        Text = textBox.Text;
        UpdatePlaceholderVisibility();
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        border.BorderBrush = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#3B82F6")
        );
        border.BorderThickness = new Thickness(1.5);
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        border.BorderBrush = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#D1D5DB")
        );
        border.BorderThickness = new Thickness(1);
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonClick?.Invoke(this, e);
    }

    #endregion

    #region Methods

    private void UpdatePlaceholderVisibility()
    {
        IsPlaceholderVisible = string.IsNullOrEmpty(Text);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Clear()
    {
        Text = string.Empty;
        textBox.Clear();
    }

    public new void Focus()
    {
        textBox.Focus();
    }

    #endregion
}
