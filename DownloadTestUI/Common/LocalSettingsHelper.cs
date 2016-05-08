using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DownloadTestUI.Common
{
    public class LocalSettingsHelper
    {
        public static void AddOrUpdateValue(string Key, Object Value)
        {
            ApplicationData.Current.LocalSettings.Values[Key] = Value;
        }

        public static T GetValueOrDefault<T>(string Key, T DefaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains(Key))
            {
                value = (T)ApplicationData.Current.LocalSettings.Values[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = DefaultValue;
            }

            return value;
        }
    }
}
