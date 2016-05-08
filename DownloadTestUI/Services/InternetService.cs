using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DownloadTestUI.Services
{
    public class InternetService : IInternetService
    {
        private Worker worker;

        public InternetService()
        {
            worker = new Worker();
        }

        public async Task<TestResult> Get(Uri uri)
        {
            var result = await worker.Get(uri);
            return result;
        }

        public async Task<HttpStatusCode> PostResult(Uri uri, TestResult result, string clientId)
        {
            var httpStatus = await worker.PostResult(uri, result, clientId);
            return httpStatus;
        }
    }
}
