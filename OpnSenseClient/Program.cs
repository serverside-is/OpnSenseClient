using OpnSenseClient;
using OpnSenseClient.Models;
using OpnSenseClient.OpnSenseApiServers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpnSenseClient.WireGuardApi;
using System.ComponentModel;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var wireguardClients = new List<WireGuardClient>();

            var opnSenseServers = OpnSenseApiServers.GetOpnSenseApiClients();

            foreach (var opnSenseServer in opnSenseServers)
            {
                var clients = await WireGuardApi.GetWireGuardClientsAsync(opnSenseServer);
                foreach (var client in clients)
                {
                    wireguardClients.Add(client);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
}
