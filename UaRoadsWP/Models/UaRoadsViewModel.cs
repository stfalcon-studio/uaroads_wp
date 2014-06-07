using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using UaRoadsWpApi;

namespace UaRoadsWP.Models
{
    public class UaRoadsViewModel : BaseViewModel
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }


        public ICommand NavigateToAllTacksPageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NavigationService.Navigate(new Uri("/Pages/TracksPage.xaml", UriKind.Relative));
                });
            }
        }


        public Task<ApiResponse> RegisterDevice(string email)
        {
            return new ApiClient().Login(email
                , Environment.OSVersion.Platform.ToString()
                , Microsoft.Phone.Info.DeviceStatus.DeviceName
                , Environment.OSVersion.Version.ToString()
                , Windows.Phone.System.Analytics.HostInformation.PublisherHostId);
        }
    }


    public class BaseViewModel : ViewModelBase
    {
        public PhoneApplicationFrame RootFrame
        {
            get { return (Microsoft.Phone.Controls.PhoneApplicationFrame)Application.Current.RootVisual; }
        }

        public PhoneApplicationPage RootPage
        {
            get { return (PhoneApplicationPage)((Microsoft.Phone.Controls.PhoneApplicationFrame)Application.Current.RootVisual).Content; }
        }

        public NavigationService NavigationService
        {
            get { return RootPage.NavigationService; }
        }


        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
        }
    }
}