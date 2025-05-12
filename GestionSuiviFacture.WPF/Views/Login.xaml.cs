using System.Windows;
using System.Windows.Controls;
using GestionSuiviFacture.WPF.ViewModels;
using System.Windows.Input;

namespace GestionSuiviFacture.WPF.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            Loaded += (s, e) => {
                if (DataContext is LoginViewModel viewModel)
                {
                    PasswordInputBox.PasswordChanged += (sender, args) => {
                        viewModel.Password = PasswordInputBox.Password;
                    };
                }
            };
        }

        private void DragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.DragMove();
        }


        private void UsernameTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordInputBox.Focus();
            }
        }

        private void PasswordInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
                {
                    vm.LoginCommand.Execute(null);
                }
            }
        }
    }
}
