using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using UaRoadsWP.Models;
using UaRoadsWpApi;

namespace UaRoadsWP.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        // Constructor
        public SettingsPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ((BaseViewModel)DataContext).OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ((BaseViewModel)DataContext).OnNavigatedFrom(e);
        }
    }
}