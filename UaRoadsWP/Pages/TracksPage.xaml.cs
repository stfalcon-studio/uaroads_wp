using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

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

            (DataContext as TracksPageViewModel).OnLoaded();
        }

        private void ClearAllTracks(object sender, EventArgs e)
        {
            (DataContext as TracksPageViewModel).ClearAll(); 
        }
    }
}