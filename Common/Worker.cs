using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Worker
    {
        HttpClient _httpClient;

        public Worker()
        {

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
            _httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;
            _httpClient.DefaultRequestHeaders.CacheControl.NoStore = true;
            _httpClient.DefaultRequestHeaders.CacheControl.MaxAge = new TimeSpan(0);
        }

        public async Task<TestResult> Get(Uri uri)
        {
            var testResult = new TestResult();
            var stopwatch = new Stopwatch();

            testResult.uri = uri;
            testResult.startTime = DateTime.Now;

            try
            {
                stopwatch.Start();
                var httpResponse = await _httpClient.GetAsync(uri);
                stopwatch.Stop();

                testResult.httpResponse = httpResponse.StatusCode;
                testResult.message = httpResponse.ReasonPhrase;
                testResult.duration = stopwatch.Elapsed;

                var response = await httpResponse.Content.ReadAsByteArrayAsync();
                testResult.sizeInBytes = response.Length;
            }
            catch (Exception ex)
            {
                testResult.message = ex.Message;
            }

            return testResult;
        }

        public async Task<HttpStatusCode> PostResult(Uri uri, TestResult result, string clientId)
        {
            result.ClientId = clientId;

            // TODO: check if the Uri is IFFT, then use maker channel format for json body instead (value1=, value2= etc.)
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(result.ToJsonString()));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(uri, content);

            return response.StatusCode;
        }
    }
}
