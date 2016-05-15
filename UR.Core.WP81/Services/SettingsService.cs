using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eve.Core;

namespace UR.Core.WP81.Services
{
    public class SettingsService
    {
        private static IApplicationLocalSettings _localSettings = new ApplicationLocalSettingsService();
        private static IApplicationRoamingSettings _roamingSettings = new ApplicationRoamingSettingsService();

        //public SettingsService(IApplicationLocalSettings localSettings, IApplicationRoamingSettings roamingSettings)
        //{
        //    _localSettings = localSettings;
        //    _roamingSettings = roamingSettings;
        //}

        public static Guid CurrentTrackId
        {
            get { return _localSettings.GetSetting("UaRoads.CurrentTrackId", Guid.Empty); }
            set { _localSettings.SetSetting("UaRoads.CurrentTrackId", value); }
        }

        public static string DeviceId
        {
            get { return _localSettings.GetSetting("UaRoads.DeviceId", String.Empty); }
            set { _localSettings.SetSetting("UaRoads.DeviceId", value); }
        }

        public static string UserEmail
        {
            get { return _localSettings.GetSetting("UaRoads.UserEmail", String.Empty); }
            set { _localSettings.SetSetting("UaRoads.UserEmail", value); }
        }

        public static string DeviceName
        {
            get { return _localSettings.GetSetting("UaRoads.DeviceName", String.Empty); }
            set { _localSettings.SetSetting("UaRoads.DeviceName", value); }
        }
        public static string OverrideLanguageCode
        {
            get { return _localSettings.GetSetting("UaRoads.OverrideLanguageCode", String.Empty); }
            set { _localSettings.SetSetting("UaRoads.OverrideLanguageCode", value); }
        }

        //public static AUser User
        //{
        //    get { return _localSettings.GetSetting("Megogo.User", (AUser)null); }
        //    set { _localSettings.SetSetting("Megogo.User", value); }
        //}

        //public static bool IsWelcomeScreenShown
        //{
        //    get { return _localSettings.GetSetting("Megogo.IsWelcomeScreenShown", false); }
        //    set { _localSettings.SetSetting("Megogo.IsWelcomeScreenShown", value); }
        //}
    }
}
