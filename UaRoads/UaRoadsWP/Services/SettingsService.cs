
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
        public static string LastUserLogin
        {
            get { return GetService().GetSetting("LastUserLogin", String.Empty); }
            set { GetService().SetSetting("LastUserLogin", value); }
        }

        public static ApplicationSettingsService GetService()
        {
            return new ApplicationSettingsService();
        }
    }
}
