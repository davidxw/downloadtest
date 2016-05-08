using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTestUI.Services
{
    public interface IInternetService
    {
        Task<TestResult> Get(Uri uri);

        Task<HttpStatusCode> PostResult(Uri uri, TestResult result, string clientId);
    }
}
