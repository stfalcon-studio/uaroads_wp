
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaRoadsWP.Base;

namespace UaRoadsWP.Services
{
    public static class SettingsService
    {
        public static string UserLogin
        {
            get { return GetService().GetSetting("UserLogin", String.Empty); }
            set { GetService().SetSetting("UserLogin", value); }
        }

        public static Guid LastRecordedRoad
        {
            get { return GetService().GetSetting("LastRecordedRoad", Guid.Empty); }
            set { GetService().SetSetting("LastRecordedRoad", value); }
        }

        public static short? CurrentTrack
        {
            get { return GetService().GetSetting("CurrentTrack", (short?)null); }
            set { GetService().SetSetting("CurrentTrack", value); }
        }

        public static ApplicationSettingsService GetService()
        {
            return new ApplicationSettingsService();
        }
    }
}
