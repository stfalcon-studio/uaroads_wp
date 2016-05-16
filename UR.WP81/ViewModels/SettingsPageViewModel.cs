using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Caliburn.Micro;
using Eve.Caliburn;
using UR.Core.WP81;
using UR.Core.WP81.Common;
using UR.Core.WP81.Services;
using UR.WP81.Common;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class SettingsPageViewModel : AppBasePageViewModel
    {
        private string _selectedLanguageCode;

        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Languages = AppConstant.AppLanguages;

            Languages.First(x => string.IsNullOrEmpty(x.Code)).Name = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/SettingsPageViewModel_DefaultLanguage").ValueAsString;

            _selectedLanguageCode = SettingsService.OverrideLanguageCode;
        }

        public List<AppLanguage> Languages { get; set; }

        public string SelectedLanguageCode
        {
            get { return _selectedLanguageCode; }
            set
            {
                if (Equals(value, _selectedLanguageCode)) return;
                _selectedLanguageCode = value;
                ChangeLanguage(_selectedLanguageCode);

                NotifyOfPropertyChange(() => SelectedLanguageCode);
            }
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
            Launcher.LaunchUriAsync(new Uri("http://uaroads.com/"));
        }

        public void GoToPrivacy()
        {
            NavigationService.UriFor<PrivacyPageViewModel>().Navigate();
        }


        private async void ChangeLanguage(string code)
        {
            SettingsService.OverrideLanguageCode = code;
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = code;

            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();

            await Task.Delay(200);

            NavigationService.CleanNavigationStackAfter().UriFor<SplashScreenPageViewModel>().Navigate();
        }
    }
}
