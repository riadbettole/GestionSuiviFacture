using System.Windows;
using System.Windows.Controls;

namespace GestionSuiviFacture.WPF.Components
{
    public partial class LayoutPageButton : UserControl
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(LayoutPageButton), new PropertyMetadata(string.Empty));

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(LayoutPageButton), new PropertyMetadata(string.Empty));

        public LayoutPageButton()
        {
            InitializeComponent();
        }
    }
}
