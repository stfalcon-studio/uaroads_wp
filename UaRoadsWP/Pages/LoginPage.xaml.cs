using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using UaRoadsWP.Models;

namespace UaRoadsWP.Pages
{
    public partial class LoginPage : PhoneApplicationPage
    {
        // Constructor
        public LoginPage()
        {
            InitializeComponent();
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