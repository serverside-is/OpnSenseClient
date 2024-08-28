using OpnSenseClient.OpnSenseApiServers;
using OpnSenseClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpnSenseClient.WireGuardApi
{
    internal static class WireGuardApi
    {
        public async static Task<List<WireGuardClient>> GetWireGuardClientsAsync(OpnSenseApiServer opnSenseServer)
        {
            var response = await opnSenseServer.HttpClient.GetAsync("wireguard/client/get");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching clients: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var clientsList = new List<WireGuardClient>();

            try
            {
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    var root = document.RootElement;

                    // Navigate to the inner "client" dictionary
                    if (root.TryGetProperty("client", out JsonElement clientElement) &&
                        clientElement.TryGetProperty("clients", out JsonElement clientsElement) &&
                        clientsElement.TryGetProperty("client", out JsonElement innerClientElement))
                    {
                        // Deserialize the inner "client" dictionary into a Dictionary<string, WireGuardClient>
                        var clientsDict = JsonSerializer.Deserialize<Dictionary<string, WireGuardClient>>(innerClientElement.GetRawText());

                        if (clientsDict != null)
                        {
                            foreach (var kvp in clientsDict)
                            {
                                // Set the Id property with the dictionary key
                                kvp.Value.Id = kvp.Key;
                                // And the server this belongs to
                                kvp.Value.ServerId = opnSenseServer.Id;
                                clientsList.Add(kvp.Value);
                            }
                        }
                    }

                    return clientsList;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                throw;
            }
        }
    }
}
