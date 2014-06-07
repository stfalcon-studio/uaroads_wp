using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using UaRoadsWP.Models;

namespace UaRoadsWP.Pages
{
    public partial class TracksPage : PhoneApplicationPage
    {
        // Constructor
        public TracksPage()
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

        private void ClearAllTracks(object sender, EventArgs e)
        {
            (DataContext as TracksPageViewModel).ClearAll(); 
        }
    }
}