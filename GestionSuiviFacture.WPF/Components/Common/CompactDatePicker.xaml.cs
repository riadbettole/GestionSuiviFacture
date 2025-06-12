using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Components.Common;

public partial class CompactDatePicker : UserControl
{
    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTime?),
        typeof(CompactDatePicker),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnSelectedDateChanged
        )
    );

    public static readonly RoutedEvent DatePickerLoadedEvent = EventManager.RegisterRoutedEvent(
        nameof(DatePickerLoaded),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(CompactDatePicker)
    );

    public CompactDatePicker()
    {
        InitializeComponent();
        PART_DatePicker.Loaded += OnDatePickerLoaded;
        PART_DatePicker.SelectedDateChanged += OnInternalDatePickerSelectedDateChanged;

        this.PreviewKeyDown += OnPreviewKeyDown;
    }

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public event RoutedEventHandler DatePickerLoaded
    {
        add => AddHandler(DatePickerLoadedEvent, value);
        remove => RemoveHandler(DatePickerLoadedEvent, value);
    }

    public new bool Focus()
    {
        if (
            PART_DatePicker.Template?.FindName("PART_TextBox", PART_DatePicker)
            is DatePickerTextBox textBox
        )
        {
            return textBox.Focus();
        }
        return PART_DatePicker.Focus();
    }

    private static void OnSelectedDateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
    )
    {
        if (d is CompactDatePicker picker)
        {
            picker.PART_DatePicker.SelectedDate = picker.SelectedDate;
        }
    }

    private void OnInternalDatePickerSelectedDateChanged(
        object? sender,
        SelectionChangedEventArgs e
    )
    {
        if (PART_DatePicker.SelectedDate != SelectedDate)
        {
            SelectedDate = PART_DatePicker.SelectedDate;
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Let the parent handle PreviewKeyDown normally - don't interfere
        // The event will bubble up naturally to your DateInputBox_PreviewKeyDown handler
    }

    private void OnDatePickerLoaded(object sender, RoutedEventArgs e)
    {
        var args = new RoutedEventArgs(DatePickerLoadedEvent);
        RaiseEvent(args);
    }
}
