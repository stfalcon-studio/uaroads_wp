using System;
using Windows.System;
using Caliburn.Micro;
using UR.Core.WP81.Services;
using UR.WP81.Common;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class SettingsPageViewModel : AppBasePageViewModel
    {
        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        public string UserLogin
        {
            get { return SettingsService.UserEmail; }
        }

        public string DeviceId
        {
            get { return SettingsService.DeviceId; }
        }

        public string DeviceName
        {
            get { return SettingsService.DeviceName; }
        }

        public bool CanLogout
        {
            get { return StateService.Instance.DeviceIsRegistred; }
        }


        public void LogoutButton()
        {
            SettingsService.DeviceId = String.Empty;
            SettingsService.DeviceName = String.Empty;
            SettingsService.UserEmail = String.Empty;
            NavigationService.ToMainPage();
        }

        public void GoToWww()
        {
            //
            Launcher.LaunchUriAsync(new Uri("http://uaroads.com/"));
        }

        public void GoToPrivacy()
        {
            NavigationService.UriFor<PrivacyPageViewModel>().Navigate();
        }
    }
}
