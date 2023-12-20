using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using HealthChecker;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        string configFilePath = "/Users/zeeshan/Projects/HealthChecker/HealthChecker/yamlSample.yaml";

        try
        {
            List<Endpoint> endpoints = LoadEndpoints(configFilePath);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
            };

            while (true)
            {
                TestAndLogEndpoints(endpoints);
                Thread.Sleep(15000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static List<Endpoint> LoadEndpoints(string configFilePath)
    {
        using (var reader = new StreamReader(configFilePath))
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<List<Endpoint>>(reader);
        }
    }

    private static void TestAndLogEndpoints(List<Endpoint> endpoints)
    {
        foreach (var endpoint in endpoints)
        {
            TestEndpoint(endpoint);
        }

        LogAvailability(endpoints);
    }

    private static void TestEndpoint(Endpoint endpoint)
    {
        try
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod(endpoint.Method ?? "GET");
                request.RequestUri = new Uri(endpoint.Url);

                if (endpoint.Headers != null)
                {
                    foreach (var header in endpoint.Headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (!string.IsNullOrEmpty(endpoint.Body))
                {
                    request.Content = new StringContent(endpoint.Body, Encoding.UTF8, "application/json");
                }

                var response = httpClient.SendAsync(request).Result;

                endpoint.Availability = IsEndpointUp(response);
            }
        }
        catch
        {
            endpoint.Availability = false;
        }
    }

    private static bool IsEndpointUp(HttpResponseMessage response)
    {
        return response.IsSuccessStatusCode && response.ReasonPhrase == "OK" && response.Latency() < 500;
    }

    private static void LogAvailability(List<Endpoint> endpoints)
    {
        var availabilityByDomain = new Dictionary<string, List<bool>>();

        foreach (var endpoint in endpoints)
        {
            var domain = new Uri(endpoint.Url).Host;

            if (!availabilityByDomain.ContainsKey(domain))
            {
                availabilityByDomain[domain] = new List<bool>();
            }

            availabilityByDomain[domain].Add(endpoint.Availability);
        }

        foreach (var kvp in availabilityByDomain)
        {
            var domain = kvp.Key;
            var availabilityPercentage = Math.Round((double)kvp.Value.Count(up => up) / kvp.Value.Count * 100);

            Console.WriteLine($"{domain} has {availabilityPercentage}% availability percentage");
        }
    }
}

