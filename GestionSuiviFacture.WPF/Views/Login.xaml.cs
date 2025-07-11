﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestionSuiviFacture.WPF.ViewModels;

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
}
