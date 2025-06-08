using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GestionSuiviFacture.WPF.Components.Common
{
    /// <summary>
    /// Interaction logic for CompactDatePicker.xaml
    /// </summary>

    public partial class CompactDatePicker : UserControl, INotifyPropertyChanged
    {
        public CompactDatePicker()
        {
            InitializeComponent();
            Loaded += CompactDatePicker_Loaded;
        }

        #region Dependency Properties

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(CompactDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(CompactDatePicker),
                new PropertyMetadata("Select date..."));

        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        #endregion

        #region Events

        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged;
        public event KeyEventHandler DatePickerKeyDown;
        public event PropertyChangedEventHandler PropertyChanged;

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

        private void CompactDatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
            SyncCalendarWithSelectedDate();
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CompactDatePicker datePicker)
            {
                datePicker.UpdateDatePickerText();
                datePicker.UpdatePlaceholderVisibility();
                datePicker.SyncCalendarWithSelectedDate();

                datePicker.SelectedDateChanged?.Invoke(datePicker,
                    new SelectionChangedEventArgs(Selector.SelectionChangedEvent,
                        e.OldValue != null ? new object[] { e.OldValue } : new object[0],
                        e.NewValue != null ? new object[] { e.NewValue } : new object[0]));
            }
        }

        private void DatePickerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
            border.BorderThickness = new Thickness(1.5);
        }

        private void DatePickerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D5DB"));
            border.BorderThickness = new Thickness(1);

            // Try to parse the entered text
            if (DateTime.TryParse(datePickerTextBox.Text, out DateTime parsedDate))
            {
                SelectedDate = parsedDate;
            }
            else if (string.IsNullOrWhiteSpace(datePickerTextBox.Text))
            {
                SelectedDate = null;
            }
        }

        private void DatePickerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Fire the optional KeyDown event
            DatePickerKeyDown?.Invoke(this, e);

            // Handle Enter key to parse date
            if (e.Key == Key.Enter)
            {
                if (DateTime.TryParse(datePickerTextBox.Text, out DateTime parsedDate))
                {
                    SelectedDate = parsedDate;
                }
                datePickerTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            calendarPopup.IsOpen = !calendarPopup.IsOpen;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar.SelectedDate.HasValue)
            {
                SelectedDate = calendar.SelectedDate.Value;
                calendarPopup.IsOpen = false;
            }
        }

        #endregion

        #region Methods

        private void UpdatePlaceholderVisibility()
        {
            IsPlaceholderVisible = !SelectedDate.HasValue && string.IsNullOrWhiteSpace(datePickerTextBox.Text);
        }

        private void UpdateDatePickerText()
        {
            if (SelectedDate.HasValue)
            {
                datePickerTextBox.Text = SelectedDate.Value.ToString("d"); // Short date format
            }
            else
            {
                datePickerTextBox.Text = string.Empty;
            }
        }

        private void SyncCalendarWithSelectedDate()
        {
            calendar.SelectedDate = SelectedDate;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Focus()
        {
            datePickerTextBox.Focus();
        }

        #endregion
    }
}

    
