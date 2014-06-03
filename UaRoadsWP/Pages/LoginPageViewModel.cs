using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UaRoadsWP.Models;
using UaRoadsWP.Services;

namespace UaRoadsWP.Pages
{
    public class LoginPageViewModel : UaRoadsViewModel
    {
        public string UserEmail { get; set; }

        public ICommand OkCommand { get; set; }


        public LoginPageViewModel()
        {
            OkCommand = new RelayCommand(() =>
            {
                if (IsBusy) return;
                IsBusy = true;

                if (!CheckEmail(UserEmail))
                {
                    MessageBox.Show("Check your message box and try again");
                }
                else
                {
                    SettingsService.UserLogin = UserEmail;

                    IsBusy = false;

                    RootFrame.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
                }

                IsBusy = false;
            });
        }


        public static bool CheckEmail(string email)
        {
            if (email.Contains("@") & email.Contains(".")) return true;

            return false;
        }
    }
}
