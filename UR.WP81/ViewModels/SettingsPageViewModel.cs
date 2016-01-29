using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using UR.Core.WP81.API.Models;
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
    }
}
