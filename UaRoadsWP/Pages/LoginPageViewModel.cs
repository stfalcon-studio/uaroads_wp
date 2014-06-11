using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UaRoadsWP.Models;
using UaRoadsWP.Services;
using UaRoadsWpApi;


namespace UaRoadsWP.Pages
{
    public class LoginPageViewModel : UaRoadsViewModel
    {
        public string UserEmail { get; set; }

        public ICommand OkCommand { get; set; }

        public ICommand SkipLoginCommand { get; set; }

        public LoginPageViewModel()
        {
            UserEmail = "bondarenkod@windowslive.com";

            OkCommand = new RelayCommand(async () =>
            {
                if (IsBusy) return;
                IsBusy = true;

                if (!CheckEmail(UserEmail))
                {
                    MessageBox.Show("Check your email and try again");
                }
                else
                {
                    var res = await RegisterDevice(UserEmail);

                    IsBusy = false;

                    if (ApiResponseProcessor.Process(res))
                    {
                        SettingsService.UserLogin = UserEmail;

                        RootFrame.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
                    }
                }

                IsBusy = false;
            });


            SkipLoginCommand = new RelayCommand(SkipLoginExecute);
        }

        private void SkipLoginExecute()
        {
            SettingsService.DelayLogin = true;
            RootFrame.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }


        public static bool CheckEmail(string email)
        {
            if (email.Contains("@") & email.Contains(".")) return true;

            return false;
        }
    }
}
