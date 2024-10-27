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
            return new List<OpnSenseApiServer>
            {
                new OpnSenseApiServer("opnsense-40-0", "vpn.serverside.pt", "ZkcSgRVkeE1e8H2Uld0pWUYqnJh727hcBgpXpPGu0sUw0G/4MapsVMOCV6K3MYEH7CP+cM2CdM6vGSo/", "cNkULcNljencmMTVyD313/KOh7R3/8103TPDBMb1k3YlSDDor6iOX/ovzt3w/2t5xBdJdq4c8xVVBJ8w"),
                new OpnSenseApiServer("opnsense-40-16", "vpn.serverside.pt", "AwV4bAGR+NkYlCqrA5y8XP+HetxKwAIHybr3mRYdSycUK9V61S0hpGsf5dTkp6XrmswM4OT83HVH70Z7", "XO6MDt4XnHBoU4nC2CYUsd463MDGoMgXsMS7M5Fd/gHm4RlBf4CB5TExnSFBFhR4EouTflHk14xO3fKT"),
                new OpnSenseApiServer("opnsense-10-16", "vpn.mediapost.pt", "UFpq/fd+wwotbf7actgHrjROycBrOgNe1g913aah3/R6jOYo8yjKGSRun17gv35Fy7NRFgxZYOHi7AHM", "iQ/hPaebzPPcKvzlVkC9yxNGVw6uZ6Oa94D05iODS4jXW4GQDftULHp4i0D9qk5TubSQTZ051RfE8E2x"),
                new OpnSenseApiServer("opnsense-10-24", "vpn.mediapost.pt", "vJ2dAF6qVuz6QnqcFV9xFW/idVfVI2su7Sp+44fKcLFpE9UekfOHyyIq8Ma9K79juIZDoIOcjrBOSe4P", "1CfZ2PEQ+N6ctb6Dmf6QsGlu4TCILqqKL6JmsfDmPbZZXWH3bgL6NxjQy76MdI3PkGEjDYKnjjHLooqF"),
                new OpnSenseApiServer("opnsense-10-200", "vpn.mediapost.pt", "n4VLT0EIBWHy5WYD7Aj+VPf5qxjwuDNaJQkPVVAuzNHfx8D4OLYMMJdh+pXf7/ZpSND6CldKJ/ikmcuX", "UUPrieaRkBSORqvb+kB5HoiqZgAlsXQdfuTWOgMjbkstg505l4ceEbn+sJZLwWan2a2xX0PC1wIJyXb7"),
                new OpnSenseApiServer("opnsense-10-208", "vpn.mediapost.pt", "uPLY0z9NYCtSteM858/SeqtAk+QiwgwvhP+uTrRsvMi+nbVvW0ipRi3IDmXwHgX3wzO8vvSv00imvgRJ", "jOh/GJXDexhfqHqY6yx/zPGrI/iS0Ov5pTEJpR92+JEg9YtvM58vvQfU0BK/UL5ooctqlJRTURgjsqoK")
            }.FindAll(server => server.Id.Contains(filter));
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
