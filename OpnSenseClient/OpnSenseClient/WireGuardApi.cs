using OpnSenseClient.Models;
using OpnSenseClient.OpnSenseApiServers;
using System.Net.Http.Json;
using System.Text.Json;

internal static class WireGuardApi
{
    public async static Task<List<WireGuardClient>> GetWireGuardClientsAsync(OpnSenseApiServer opnSenseServer, string filter = "")
    {
        var payload = new { current = 1, rowCount = -1, sort = "{}", searchPhrase = filter };
        var response = await opnSenseServer.HttpClient.PostAsJsonAsync("wireguard/client/searchClient", payload);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching clients: {response.StatusCode} - {response.ReasonPhrase}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var clientsList = new List<WireGuardClient>();

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            var rows = document.RootElement.GetProperty("rows");

            foreach (var rowProperty in rows.EnumerateArray())
            {
                WireGuardClient? client = JsonSerializer.Deserialize<WireGuardClient>(rowProperty.GetRawText());

                if (client != null)
                {
                    // Add the opnsense server this belongs to
                    client.ServerId = opnSenseServer.Id;
                    // Add the uuid of the wireguard server this client belongs to
                    client.ServerUuid = opnSenseServer.Servers.Find(server => server.Name == client.WireGuardServerUuid)?.Uuid ?? "";

                    clientsList.Add(client);
                }
            }

            return clientsList;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
            throw;
        }
    }

    public async static Task<List<WireGuardServer>> GetWireGuardServersAsync(OpnSenseApiServer opnSenseServer)
    {
        var response = await opnSenseServer.HttpClient.GetAsync("wireguard/client/listServers");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching servers: {response.StatusCode} - {response.ReasonPhrase}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var serversList = new List<WireGuardServer>();

        try
        {
            var responseObject = JsonDocument.Parse(json).RootElement;

            if (responseObject.TryGetProperty("rows", out var rows) && rows.ValueKind == JsonValueKind.Array)
            {
                foreach (var row in rows.EnumerateArray())
                {
                    if (row.TryGetProperty("uuid", out var uuid) && row.TryGetProperty("name", out var name))
                    {
                        var server = new WireGuardServer
                        {
                            Uuid = uuid.ToString(),
                            Name = name.ToString()
                        };

                        serversList.Add(server);
                    }
                }
            }

            return serversList;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
            throw;
        }
    }

    public async static Task SetWireGuardPeerAsync(OpnSenseApiServer opnSenseServer, WireGuardClient client)
    {
        var payload = new
        {
            client = new
            {
                enabled = client.Enabled,
                name = client.Name,
                pubkey = client.PubKey,
                psk = "",
                tunneladdress = client.TunnelAddress,
                serveraddress = client.ServerAddress,
                serverport = client.ServerPort,
                servers = client.WireGuardServerUuid,
                keepalive = ""
            }
        };

        string endpoint = string.IsNullOrEmpty(client.Uuid) ? "wireguard/client/addClient" : $"wireguard/client/setClient/{client.Uuid}";

        var response = await opnSenseServer.HttpClient.PostAsJsonAsync(endpoint, payload);

        var errorResponse = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(errorResponse);
        var resultProperty = json.RootElement.GetProperty("result");

        if (resultProperty.GetString() != "saved")
        {
            throw new HttpRequestException($"Error {(string.IsNullOrEmpty(client.Uuid) ? "creating" : "updating")} wireguard peer: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }
}
