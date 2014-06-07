using System;
using System.Windows.Navigation;
using UaRoadsWP.Models;
using UaRoadsWP.Services;
using UaRoadsWpApi;

namespace UaRoadsWP.Pages
{
    public class StartPageViewModel : UaRoadsViewModel
    {
        public StartPageViewModel()
        {

        }

        public async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //var res = await RegisterDevice("bondarenkod@windowslive.com");

            //ApiResponseProcessor.Process(res);

            //return;


            if (String.IsNullOrEmpty(SettingsService.UserLogin))
            {
                this.NavigationService.Navigate(new Uri("/Pages/LoginPage.xaml", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}
