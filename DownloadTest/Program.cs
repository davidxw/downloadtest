using Common;
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
        private static Worker _timedHttpRequest = new Worker();

        static void Main(string[] args)
        {
            // arg[0] - URL
            // arg[1] - wait time in seconds, zero for single attempt
            // arg[2] - optional, log file
            // arg[3] - http endpoint to POST results to

            Uri testUri = null;
            int waitTimeSeconds;
            string logFilePath;
            Uri resultUri = null;

            ValidateArgs(args, out testUri, out waitTimeSeconds, out logFilePath, out resultUri);

            if (waitTimeSeconds == 0)
            {
                RunOnce(testUri, logFilePath, resultUri).Wait();
#if DEBUG
                Console.WriteLine("Press Enter ...");
                Console.ReadKey();
#endif
            }
            else
            {
                while (true)
                {
                    RunOnce(testUri, logFilePath, resultUri).Wait();
                    Thread.Sleep(waitTimeSeconds * 1000);
                }
            }
        }

        private static async Task RunOnce(Uri testUri, string logFilePath, Uri resultUri)
        {
            var testResult = await _timedHttpRequest.Get(testUri);

            if (string.IsNullOrEmpty(logFilePath))
            {
                Console.WriteLine($"{testResult}");
            }
            else
            {
                File.AppendAllText(logFilePath, $"{testResult}{Environment.NewLine}");
            }

            if (resultUri != null)
            {
                var resultsPostResponse = await _timedHttpRequest.PostResult(resultUri, testResult);

                Console.WriteLine($"Results posted to {resultUri} - Http Status: {resultsPostResponse}");
            }
        }


        private static void ValidateArgs(string[] args, out Uri testUri, out int waitTime, out string logFilePath, out Uri resultsUri)
        {
            if (args.Count() < 2 || args.Count() > 4)
            {
                WriteUsageAndQuit();
            }

            Uri.TryCreate(args[0], UriKind.Absolute, out testUri);

            if (testUri == null)
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

                if (!string.IsNullOrEmpty(logFilePath))
                {
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
            }
            else
            {
                logFilePath = null;
            }

            if (args.Count() > 3)
            {
                Uri.TryCreate(args[3], UriKind.Absolute, out resultsUri);

                if (resultsUri == null)
                {
                    Console.WriteLine("Failed to convert results URL paramater to URL.");
                    WriteUsageAndQuit();
                }
            }
            else
            {
                resultsUri = null;
            }
        }

        private static void WriteUsageAndQuit()
        {
            Console.WriteLine($"Usage: DownloadTest.exe URL waitTime {{logFilePath}} {{httpEndPoint}}");
            Console.WriteLine($"       URL: Valid URL with no authentication. Should be of a reasonably large size (e.g. an image).");
            Console.WriteLine($"       waitTime: time in seconds between gets.  Use zero to get one time and then exit.");
            Console.WriteLine($"       logFilePath (optional): file for results. If not specified or blank then results are written to console.");
            Console.WriteLine($"       httpEndPoint (optional): a URL to POST result to.  Results will be posted as JSON");

            Console.WriteLine($"Output columns:");
            Console.WriteLine($"       dateTime, url, status message, http response, duration in milliseconds, size in byes, speed in Mbps");

#if DEBUG
            Console.WriteLine("Press Enter ...");
            Console.ReadKey();
#endif
            System.Environment.Exit(-1);
        }
    }
}
