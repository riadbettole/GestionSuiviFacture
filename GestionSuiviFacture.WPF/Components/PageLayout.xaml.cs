using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Components
{
    /// <summary>
    /// Interaction logic for PageLayout.xaml
    /// </summary>
    public partial class PageLayout : UserControl
    {

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(PageLayout), new PropertyMetadata(string.Empty));


        public PageLayout()
        {
            InitializeComponent();
        }

        private void DragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.DragMove();
        }
    }
}
