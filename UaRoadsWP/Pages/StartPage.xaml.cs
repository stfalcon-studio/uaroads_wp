using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using UaRoadsWP.Services;

namespace UaRoadsWP.Pages
{
    public partial class StartPage : PhoneApplicationPage
    {
        // Constructor
        public StartPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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