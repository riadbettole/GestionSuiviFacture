//using System.Text.RegularExpressions;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Input;

//public static class DatePickerMaskBehavior
//{
//    public static readonly DependencyProperty UseMaskProperty =
//        DependencyProperty.RegisterAttached("UseMask", typeof(bool), typeof(DatePickerMaskBehavior),
//            new PropertyMetadata(false, OnUseMaskChanged));

//    public static bool GetUseMask(DependencyObject obj)
//    {
//        return (bool)obj.GetValue(UseMaskProperty);
//    }

//    public static void SetUseMask(DependencyObject obj, bool value)
//    {
//        obj.SetValue(UseMaskProperty, value);
//    }

//    private static void OnUseMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        if (d is DatePicker datePicker)
//        {
//            if ((bool)e.NewValue)
//            {
//                datePicker.Loaded += DatePicker_Loaded;
//            }
//            else
//            {
//                datePicker.Loaded -= DatePicker_Loaded;
//            }
//        }
//    }

//    private static void DatePicker_Loaded(object sender, RoutedEventArgs e)
//    {
//        var datePicker = sender as DatePicker;
//        var textBox = GetDatePickerTextBox(datePicker);

//        if (textBox != null)
//        {
//            textBox.PreviewTextInput += TextBox_PreviewTextInput;
//            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
//            textBox.TextChanged += TextBox_TextChanged;
//            textBox.GotFocus += TextBox_GotFocus;

//            // Initialize with format if empty
//            if (string.IsNullOrEmpty(textBox.Text))
//            {
//                textBox.Text = "dd/mm/yyyy";
//                textBox.Foreground = System.Windows.Media.Brushes.Gray;
//            }
//        }
//    }

//    private static DatePickerTextBox GetDatePickerTextBox(DatePicker datePicker)
//    {
//        return datePicker.Template?.FindName("PART_TextBox", datePicker) as DatePickerTextBox;
//    }

//    private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
//    {
//        var textBox = sender as DatePickerTextBox;
//        if (textBox.Text == "dd/mm/yyyy")
//        {
//            textBox.Text = "__/__/____";
//            textBox.CaretIndex = 0;
//            textBox.Foreground = System.Windows.Media.Brushes.Black;
//        }
//    }

//    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
//    {
//        var textBox = sender as DatePickerTextBox;

//        // Only allow digits
//        if (!char.IsDigit(e.Text[0]))
//        {
//            e.Handled = true;
//            return;
//        }

//        var currentText = textBox.Text;
//        var caretIndex = textBox.CaretIndex;

//        // Skip over slashes
//        if (caretIndex == 2 || caretIndex == 5)
//        {
//            caretIndex++;
//            textBox.CaretIndex = caretIndex;
//        }

//        // Don't allow input beyond the format length
//        if (caretIndex >= 10)
//        {
//            e.Handled = true;
//            return;
//        }

//        // Replace the character at current position
//        var newText = currentText.ToCharArray();
//        newText[caretIndex] = e.Text[0];
//        textBox.Text = new string(newText);

//        // Move caret to next position, skipping slashes
//        var nextPosition = caretIndex + 1;
//        if (nextPosition == 2 || nextPosition == 5)
//            nextPosition++;

//        if (nextPosition <= 10)
//            textBox.CaretIndex = nextPosition;

//        e.Handled = true;

//        // Try to parse and set the date
//        TrySetDate(textBox);
//    }

//    private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
//    {
//        var textBox = sender as DatePickerTextBox;

//        if (e.Key == Key.Back || e.Key == Key.Delete)
//        {
//            var caretIndex = textBox.CaretIndex;

//            if (e.Key == Key.Back && caretIndex > 0)
//            {
//                // Move back, skipping slashes
//                if (caretIndex == 3 || caretIndex == 6)
//                    caretIndex--;
//                caretIndex--;

//                if (caretIndex >= 0)
//                {
//                    var newText = textBox.Text.ToCharArray();
//                    newText[caretIndex] = '_';
//                    textBox.Text = new string(newText);
//                    textBox.CaretIndex = caretIndex;
//                }
//            }
//            else if (e.Key == Key.Delete && caretIndex < textBox.Text.Length)
//            {
//                // Skip slashes for delete
//                if (caretIndex == 2 || caretIndex == 5)
//                    caretIndex++;

//                if (caretIndex < textBox.Text.Length)
//                {
//                    var newText = textBox.Text.ToCharArray();
//                    newText[caretIndex] = '_';
//                    textBox.Text = new string(newText);
//                }
//            }

//            e.Handled = true;
//            TrySetDate(textBox);
//        }
//        else if (e.Key == Key.Left || e.Key == Key.Right)
//        {
//            // Allow navigation but skip slashes
//            var caretIndex = textBox.CaretIndex;

//            if (e.Key == Key.Right)
//            {
//                if (caretIndex == 1) textBox.CaretIndex = 3;
//                else if (caretIndex == 4) textBox.CaretIndex = 6;
//            }
//            else if (e.Key == Key.Left)
//            {
//                if (caretIndex == 3) textBox.CaretIndex = 1;
//                else if (caretIndex == 6) textBox.CaretIndex = 4;
//            }
//        }
//    }

//    private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
//    {
//        var textBox = sender as DatePickerTextBox;

//        // Ensure slashes are always in place
//        if (textBox.Text.Length >= 10)
//        {
//            var chars = textBox.Text.ToCharArray();
//            chars[2] = '/';
//            chars[5] = '/';

//            var newText = new string(chars);
//            if (textBox.Text != newText)
//            {
//                var caretPos = textBox.CaretIndex;
//                textBox.Text = newText;
//                textBox.CaretIndex = Math.Min(caretPos, textBox.Text.Length);
//            }
//        }
//    }

//    private static void TrySetDate(DatePickerTextBox textBox)
//    {
//        var datePicker = GetParentDatePicker(textBox);
//        if (datePicker == null) return;

//        var text = textBox.Text.Replace("_", "");

//        // Check if we have a complete date (dd/mm/yyyy)
//        if (Regex.IsMatch(text, @"^\d{2}/\d{2}/\d{4}$"))
//        {
//            if (DateTime.TryParseExact(text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
//            {
//                datePicker.SelectedDate = date;
//                textBox.Foreground = System.Windows.Media.Brushes.Black;
//            }
//            else
//            {
//                textBox.Foreground = System.Windows.Media.Brushes.Red; // Invalid date
//            }
//        }
//        else
//        {
//            datePicker.SelectedDate = null;
//            textBox.Foreground = System.Windows.Media.Brushes.Black;
//        }
//    }

//    private static DatePicker GetParentDatePicker(FrameworkElement element)
//    {
//        var parent = element.Parent;
//        while (parent != null && !(parent is DatePicker))
//        {
//            parent = (parent as FrameworkElement)?.Parent;
//        }
//        return parent as DatePicker;
//    }
//}