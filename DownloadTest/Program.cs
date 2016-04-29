using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadTest
{
    class Program
    {
        private static HttpClient _httpClient;
        static void Main(string[] args)
        {
            // arg[0] - URL
            // arg[1] - wait time in seconds, zero for single attempt
            // arg[2] - optional, log file

            Uri uri = null;
            int waitTimeSeconds;
            string logFilePath;

            ValidateArgs(args, out uri, out waitTimeSeconds, out logFilePath);

            _httpClient = new HttpClient();

            if (waitTimeSeconds == 0)
            {
                RunOnce(uri, logFilePath).Wait();
#if DEBUG
                Console.WriteLine("Press Enter ...");
                Console.ReadKey();
#endif
            }
            else
            {
                while (true)
                {
                    RunOnce(uri, logFilePath).Wait();
                    Thread.Sleep(waitTimeSeconds * 1000);
                }
            }
        }

        private static async Task RunOnce(Uri uri, string logFilePath)
        {
            var testResult = await GetResource(uri);

            if (string.IsNullOrEmpty(logFilePath))
            {
                Console.WriteLine($"{testResult}");
            }
            else
            {
                File.AppendAllText(logFilePath, $"{testResult}{Environment.NewLine}");
            }
        }

        private static async Task<TestResult> GetResource(Uri uri)
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

        private static void ValidateArgs(string[] args, out Uri uri, out int waitTime, out string logFilePath)
        {
            if (args.Count() < 2 || args.Count() > 3)
            {
                WriteUsageAndQuit();
            }

            Uri.TryCreate(args[0], UriKind.Absolute, out uri);

            if (uri == null)
            {
                Console.WriteLine("Failed to convert first paramater to URL.");
                WriteUsageAndQuit();
            }

            if (!int.TryParse(args[1], out waitTime))
            {
                Console.WriteLine("Failed to convert second paramater to integer.");
                WriteUsageAndQuit();
            }

            if (args.Count() > 2)
            {
                logFilePath = args[2];

                try
                {
                    if (!File.Exists(logFilePath))
                    {
                        var file = File.Create(logFilePath);
                        file.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not create log file {logFilePath}: {ex.Message}");
                    WriteUsageAndQuit();
                }

            }
            else
            {
                logFilePath = null;
            }
        }

        private static void WriteUsageAndQuit()
        {
            Console.WriteLine($"Usage: DownloadTest.exe URL waitTime {{logFilePath}}");
            Console.WriteLine($"       URL: Valid URL with no authentication. Should be of a reasonably large size (e.g. an image).");
            Console.WriteLine($"       waitTime: time in seconds between gets.  Use zero to get one time and then exit.");
            Console.WriteLine($"       logFilePath (optional): file for results. If not specified then results are written to console.");
            Console.WriteLine($"Output columns:");
            Console.WriteLine($"       dateTime, url, status message, http response, duration in milliseconds, size in byes, speed in Mbps");

#if DEBUG
            Console.WriteLine("Press Enter ...");
            Console.ReadKey();
#endif
            System.Environment.Exit(-1);
        }
    }

    public class TestResult
    {
        public Uri uri { get; set; }
        public HttpStatusCode httpResponse { get; set; }
        public DateTime startTime { get; set; }
        public TimeSpan duration { get; set; }
        public decimal sizeInBytes { get; set; }
        public string message { get; set; }
        public override string ToString()
        {
            return $"{startTime},{uri},{message},{(int)httpResponse},{duration.TotalMilliseconds},{sizeInBytes},{Math.Round(mbps,2)}";
        }

        public decimal mbps
        {
            get
            {
                return ((this.sizeInBytes / 1024 / 1024) / (Decimal)(this.duration.TotalMilliseconds / 1000)) * 8;
            }
        }
    }
}
