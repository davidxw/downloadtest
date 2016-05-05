using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TestResult
    {
        public string ClientId { get; set; }
        public Uri uri { get; set; }
        public HttpStatusCode httpResponse { get; set; }
        public DateTime startTime { get; set; }
        public TimeSpan duration { get; set; }
        public decimal sizeInBytes { get; set; }
        public string message { get; set; }
        public override string ToString()
        {
            return $"{startTime},{uri},{message},{(int)httpResponse},{durationInMilliseconds},{sizeInBytes},{Math.Round(mbps, 2)}";
        }
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);        }

        public decimal mbps
        {
            get
            {
                return ((this.sizeInBytes / 1024 / 1024) / (Decimal)(this.durationInMilliseconds / 1000)) * 8;
            }
        }

        public double durationInMilliseconds
        {
            get
            {
                return duration != null ? duration.TotalMilliseconds : 0;
            }
        }

        public string startTimeSql
        {
            get
            {
                return startTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
