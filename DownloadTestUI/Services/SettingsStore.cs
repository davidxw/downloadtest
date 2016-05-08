using DownloadTestUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTestUI.Services
{
    public class SettingsStore : ISettingsStore
    {
        public string DownloadUrl
        {
            get
            {
                return LocalSettingsHelper.GetValueOrDefault<string>("DownloadUrl", null);
            }
            set
            {
                LocalSettingsHelper.AddOrUpdateValue("DownloadUrl", value);

            }
        }

        public string ResultsUrl
        {
            get
            {
                return LocalSettingsHelper.GetValueOrDefault<string>("ResultsUrl", null);
            }
            set
            {
                LocalSettingsHelper.AddOrUpdateValue("ResultsUrl", value);

            }
        }

        public TimeSpan TestInterval
        {
            get
            {
                var ticks = LocalSettingsHelper.GetValueOrDefault<long>("TestInterval", 0);

                return new TimeSpan(ticks);
            }

            set
            {
                LocalSettingsHelper.AddOrUpdateValue("ResultsUrl", value.Ticks);
            }
        }
    }
}
