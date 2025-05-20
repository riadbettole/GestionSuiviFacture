using System.Windows;
using System.Windows.Controls;

namespace GestionSuiviFacture.WPF.Components.Common
{
    /// <summary>
    /// Interaction logic for StatutDisplay.xaml
    /// </summary>
    public partial class StatutDisplay : UserControl
    {
        public StatutDisplay()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(StatutDisplay), new PropertyMetadata(""));

        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty DisplayHeightProperty =
            DependencyProperty.Register("DisplayHeight", typeof(double), typeof(StatutDisplay), new PropertyMetadata(30.0));

        public double DisplayHeight
        {
            get => (double)GetValue(DisplayHeightProperty);
            set => SetValue(DisplayHeightProperty, value);
        }

        public static readonly DependencyProperty DisplayWidthProperty =
            DependencyProperty.Register("DisplayWidth", typeof(double), typeof(StatutDisplay), new PropertyMetadata(50.0));

        public double DisplayWidth
        {
            get => (double)GetValue(DisplayWidthProperty);
            set => SetValue(DisplayWidthProperty, value);
        }

        public static readonly DependencyProperty DisplayFontSizeProperty =
            DependencyProperty.Register("DisplayFontSize", typeof(double), typeof(StatutDisplay), new PropertyMetadata(12.0));

        public double DisplayFontSize
        {
            get => (double)GetValue(DisplayFontSizeProperty);
            set => SetValue(DisplayFontSizeProperty, value);
        }
    }
}
