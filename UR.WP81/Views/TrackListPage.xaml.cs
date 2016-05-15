using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using UR.Core.WP81.Common;
using UR.WP81.ViewModels;

namespace UR.WP81.Views
{
    public sealed partial class TrackListPage : Page
    {
        public TrackListPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void TrackHold(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
            {
                var senderElement = ((FrameworkElement)e.OriginalSource);
                var menu = BuildMenu(senderElement.DataContext);
                menu.ShowAt(senderElement);
                e.Handled = true;
            }
        }

        private MenuFlyout BuildMenu(object dataContext)
        {
            var menu = new MenuFlyout();

            var menuItem = new MenuFlyoutItem
            {
                Text = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/TrackListPage_BuildMenu_ResendTrack").ValueAsString
            };


            menuItem.Click += (sender, args) =>
            {
                ((TrackListPageViewModel)DataContext).ResendTrack((ATrack)dataContext);
            };

            menu.Items.Add(menuItem);


            return menu;
        }
    }
}
