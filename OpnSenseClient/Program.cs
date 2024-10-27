using OpnSenseClient;
using OpnSenseClient.Models;
using OpnSenseClient.OpnSenseApiServers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        await SyncMediapostRoadWarriord();
        //await SyncMediapostRoadWarriordUpdate();

        // wait for a key press before closing the console
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Cria peer nos servidores que não o têm
    /// </summary>
    private static async Task SyncMediapostRoadWarriord()
    {
        try
        {
            var opnSenseServers = OpnSenseApiServers.GetOpnSenseApiClients("opnsense-10");
            opnSenseServers.Reverse(); // lixo

            var wireguardClients = new List<WireGuardClient>();
            foreach (var opnSenseServer in opnSenseServers)
            {
                var clients = await WireGuardApi.GetWireGuardClientsAsync(opnSenseServer, "rw-");
                foreach (var client in clients)
                {
                    wireguardClients.Add(client); // monte geral
                    opnSenseServer.Clients.Add(client); // os clientes deste servidor wireguard
                }
            }

            foreach (var opnSenseServer in opnSenseServers)
            {
                Console.WriteLine($"Server {opnSenseServer.Id} has {opnSenseServer.Clients.Count} clients");

                var serverUuid = opnSenseServer.Servers.Find(server => server.Name.StartsWith("wg-"));

                foreach (var client in wireguardClients)
                {
                    var peer = opnSenseServer.Clients.FirstOrDefault(c => c.PubKey == client.PubKey);

                    if (peer == null)
                    {
                        Console.WriteLine($"Creating peer {client.Name} on server {opnSenseServer.Id}");
                        Console.WriteLine($"Do you want to create this peer? (Y/n)");
                        string userInput = Console.ReadLine()?.ToLower() ?? "y";

                        if (userInput == "y" || userInput == "")
                        {

                            var newClient = new WireGuardClient
                            {
                                Enabled = "0", // client.Enabled,
                                Name = client.Name,
                                PubKey = client.PubKey,
                                TunnelAddress = client.TunnelAddress,
                                ServerAddress = client.ServerAddress,
                                ServerPort = client.ServerPort,
                                WireGuardServerUuid = serverUuid?.Uuid ?? ""
                            };

                            await WireGuardApi.SetWireGuardPeerAsync(opnSenseServer, newClient);
                        }
                    }
                }

            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Copia as definições dos peers do servidor de referência para os outros servidores
    /// </summary>
    private static async Task SyncMediapostRoadWarriordUpdate()
    {
        try
        {
            var opnSenseServers = OpnSenseApiServers.GetOpnSenseApiClients("opnsense-10");
            opnSenseServers.Reverse(); // lixo

            var wireguardClients = new List<WireGuardClient>();
            foreach (var opnSenseServer in opnSenseServers)
            {
                var clients = await WireGuardApi.GetWireGuardClientsAsync(opnSenseServer, "rw-");
                foreach (var client in clients)
                {
                    wireguardClients.Add(client); // monte geral/// <returns></returns>
                    opnSenseServer.Clients.Add(client); // os clientes deste servidor wireguard
                }
            }

            var sourceOfThruthServer = opnSenseServers.Find(server => server.Id == "opnsense-10-208");

            foreach (var opnSenseServer in opnSenseServers.Where(s => s != sourceOfThruthServer))
            {
                Console.WriteLine($"Server {opnSenseServer.Id} has {opnSenseServer.Clients.Count} clients");

                var serverUuid = opnSenseServer.Servers.Find(server => server.Name.StartsWith("wg-"));

                foreach (var client in sourceOfThruthServer!.Clients)
                {
                    var peer = opnSenseServer.Clients.FirstOrDefault(c => c.PubKey == client.PubKey);

                    if (peer != null)
                    {
                        if (peer.Name != client.Name || peer.Enabled != client.Enabled)
                        {
                            Console.WriteLine($"{client.Name} on {opnSenseServer.Id}: {peer.Name} -> {client.Name}, {peer.Enabled} -> {client.Enabled}");
                            Console.WriteLine($"Do you want to update this peer? (Y/n)");
                            string userInput = Console.ReadLine()?.ToLower() ?? "y";

                            if (userInput == "y" || userInput == "")
                            {
                                peer.Name = client.Name;
                                peer.Enabled = client.Enabled;
                                peer.WireGuardServerUuid = serverUuid?.Uuid ?? "";

                                await WireGuardApi.SetWireGuardPeerAsync(opnSenseServer, peer);
                            }
                        }
                    }
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
