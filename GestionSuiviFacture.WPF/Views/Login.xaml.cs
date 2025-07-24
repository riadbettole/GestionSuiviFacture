using GestionSuiviFacture.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GestionSuiviFacture.WPF.Views;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            if (DataContext is LoginViewModel viewModel)
            {
                PasswordInputBox.PasswordChanged += (sender, args) =>
                {
                    viewModel.Password = PasswordInputBox.Password;
                };
            }
        };
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
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
        if (
            e.Key == Key.Enter
            && DataContext is LoginViewModel vm
            && vm.LoginCommand.CanExecute(null)
        )
        {
            vm.LoginCommand.Execute(null);
        }
    }

    private void PasswordInputBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }


    private void ShowPasswordFunction()
    {
        //PasswordInputBox.Text = "HIDE";
        PasswordUnmask.Visibility = Visibility.Visible;
        PasswordInputBox.Visibility = Visibility.Hidden;
        PasswordUnmask.Text = PasswordInputBox.Password;
    }

    private void HidePasswordFunction()
    {
        //ShowPassword.Text = "SHOW";
        PasswordUnmask.Visibility = Visibility.Hidden;
        PasswordInputBox.Visibility = Visibility.Visible;
    }
    private readonly string EyePath = "M1 10c0-3.9 3.1-7 7-7s7 3.1 7 7h-1c0-3.3-2.7-6-6-6s-6 2.7-6 6H1zm4 0c0-1.7 1.3-3 3-3s3 1.3 3 3-1.3 3-3 3-3-1.3-3-3zm1 0c0 1.1.9 2 2 2s2-.9 2-2-.9-2-2-2-2 .9-2 2z";
    private readonly string EyeOffPath = "M2.49664 6.66552C3.56111 7.84827 5.18597 9 7.49998 9C9.81399 9 11.4389 7.84827 12.5033 6.66552C13.0359 6.07375 13.4219 5.48029 13.6744 5.03474C13.8003 4.81247 13.8923 4.62838 13.952 4.50163C13.9819 4.4383 14.0036 4.3894 14.0175 4.35735C14.0245 4.34133 14.0295 4.32952 14.0325 4.32225L14.0356 4.31476L14.0359 4.31397C14.0358 4.31413 14.0358 4.3143 14.5 4.5C14.9642 4.6857 14.9641 4.68591 14.9641 4.68615L14.9638 4.68671L14.9632 4.68818L14.9615 4.69248L14.9557 4.70644C14.9509 4.71808 14.944 4.73436 14.9351 4.75496C14.9172 4.79615 14.8912 4.85467 14.8566 4.92805C14.7874 5.07474 14.684 5.28129 14.5444 5.52776C14.2656 6.01971 13.839 6.67625 13.2466 7.33448C13.0638 7.53768 12.8645 7.74162 12.6483 7.94123L14.3535 9.64645L13.6464 10.3536L11.8716 8.57872C10.8638 9.30897 9.58252 9.88308 7.99998 9.98417L8 12L7 12L6.99998 9.98417C5.41745 9.88308 4.1362 9.30897 3.12838 8.57873L1.35355 10.3536L0.646443 9.64645L2.35166 7.94123C2.13551 7.74162 1.93622 7.53768 1.75334 7.33448C1.16093 6.67625 0.734378 6.01971 0.455607 5.52776C0.31594 5.28128 0.212588 5.07474 0.143435 4.92805C0.10884 4.85467 0.0827479 4.79614 0.0648994 4.75496C0.0559736 4.73436 0.0491041 4.71808 0.0442591 4.70644L0.0385039 4.69248L0.0367562 4.68818L0.0361634 4.68671L0.0359371 4.68614C0.0358434 4.68591 0.0357575 4.68569 0.499996 4.5C0.964235 4.31431 0.964164 4.31413 0.964101 4.31397L0.964012 4.31375L0.964426 4.31477L0.967513 4.32225C0.970542 4.32953 0.975513 4.34133 0.982456 4.35735C0.996346 4.3894 1.0181 4.4383 1.04796 4.50164C1.10771 4.62838 1.19967 4.81247 1.32563 5.03475C1.57811 5.48029 1.96405 6.07375 2.49664 6.66552ZM0.96397 4.31364C0.963954 4.31361 0.963968 4.31364 0.964012 4.31375L0.96397 4.31364Z";

    private void TogglePasswordButton_Checked(object sender, RoutedEventArgs e)
    {
        ShowPasswordFunction();
        EyeIcon.Data = Geometry.Parse(EyeOffPath);
    }

    private void TogglePasswordButton_Unchecked(object sender, RoutedEventArgs e)
    {
        HidePasswordFunction();
        EyeIcon.Data = Geometry.Parse(EyePath);
    }

}
