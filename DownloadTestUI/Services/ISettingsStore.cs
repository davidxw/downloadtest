using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTestUI.Services
{
    public interface ISettingsStore
    {
        string DownloadUrl { get; set; }

        string ResultsUrl { get; set; }

        TimeSpan TestInterval { get; set; }
    }
}
