using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpnSenseClient.Models;

namespace OpnSenseClient.OpnSenseApiServers
{
    public static class OpnSenseApiServers
    {
        public static List<OpnSenseApiServer> GetOpnSenseApiClients(string filter = "")
        {
            string csvFilePath = "/home/marco/VS-CODE/SERVERSIDE/opnsenseapitokens.csv";
            var servers = new List<OpnSenseApiServer>();

            try
            {
                // Read all lines from the CSV file
                var lines = File.ReadAllLines(csvFilePath);

                // Skip the header if there's one, then parse each line
                foreach (var line in lines.Skip(1))
                {
                    var fields = line.Split(',');

                    // Check if the CSV has at least 4 fields per row (adjust if more fields exist)
                    if (fields.Length >= 4)
                    {
                        var id = fields[0].Trim();
                        var host = fields[1].Trim();
                        var apiKey = fields[2].Trim();
                        var apiSecret = fields[3].Trim();

                        // Add new server entry
                        servers.Add(new OpnSenseApiServer(id, host, apiKey, apiSecret));
                    }
                }

                // Apply filter to find matching entries
                return servers.FindAll(server => server.Id.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the CSV file: {ex.Message}");
                return new List<OpnSenseApiServer>(); // Return an empty list on error
            }
        }
    }

    public class OpnSenseApiServer
    {
        public readonly string Id;
        public readonly HttpClient HttpClient;
        private readonly string ApiKey;
        private readonly string ApiSecret;
        public List<WireGuardServer> Servers = new List<WireGuardServer>();
        public List<WireGuardClient> Clients = new List<WireGuardClient>();

        public OpnSenseApiServer(string id, string domain, string apiKey, string apiSecret)
        {
            var baseUrl = $"https://{id}.{domain}:4430/api/";

            this.Id = id;
            HttpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{this.ApiKey}:{this.ApiSecret}"));
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            this.Servers = WireGuardApi.GetWireGuardServersAsync(this).Result;
        }
    }
}