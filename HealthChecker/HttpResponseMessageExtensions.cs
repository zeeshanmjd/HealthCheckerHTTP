using System;
namespace HealthChecker
{
    public static class HttpResponseMessageExtensions
    {
        public static long Latency(this HttpResponseMessage response)
        {
            return response.Headers.Date.HasValue
                ? (DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime).Milliseconds
                : 0;
        }
    }
}

