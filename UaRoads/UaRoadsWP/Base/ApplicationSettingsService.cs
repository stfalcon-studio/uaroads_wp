using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaRoadsWP.Base
{
    public interface IApplicationSettingsService
    {
        void SetSetting<T>(string key, T value);
        T GetSetting<T>(string key);
        T GetSetting<T>(string key, T defaultValue);
        bool HasSetting<T>(string key);
        bool RemoveSetting(string key);
    }


    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private static readonly object _locker = new object();

        public void SetSetting<T>(string key, T value)
        {
            lock (_locker)
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings[key] = value;
                settings.Save();
            }
        }

        public T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T));
        }

        public T GetSetting<T>(string key, T defaultValue)
        {
            lock (_locker)
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                return settings.Contains(key) &&
                       settings[key] is T
                    ? (T)settings[key]
                    : defaultValue;
            }
        }

        public bool HasSetting<T>(string key)
        {
            lock (_locker)
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                return settings.Contains(key) && settings[key] is T;
            }
        }

        public bool RemoveSetting(string key)
        {
            lock (_locker)
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains(key))
                {
                    var res = settings.Remove(key);
                    settings.Save();
                    return res;
                }
                return false;
            }
        }
    }
}
