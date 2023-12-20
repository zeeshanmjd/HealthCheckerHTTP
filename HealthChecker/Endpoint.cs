using System;
namespace HealthChecker
{
    public class Endpoint
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public bool Availability { get; set; }
    }
}

