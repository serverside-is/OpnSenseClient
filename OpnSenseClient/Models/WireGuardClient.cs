using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpnSenseClient.Models
{
    public class WireGuardClient
    {
        // This one is not mapped to JSON, because the JSON format used stinks and provides this value outside of the client data object representation
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
        [JsonIgnore]
        public string ServerId { get; set; } = string.Empty;

        [JsonPropertyName("enabled")]
        public string Enabled { get; set; } = "0";

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("pubkey")]
        public string PubKey { get; set; } = string.Empty;

        [JsonPropertyName("psk")]
        public string Psk { get; set; } = string.Empty;

        [JsonPropertyName("tunneladdress")]
        public Dictionary<string, TunnelAddressDetail> TunnelAddress { get; set; } = new Dictionary<string, TunnelAddressDetail>();

        [JsonPropertyName("serveraddress")]
        public string ServerAddress { get; set; } = string.Empty;

        [JsonPropertyName("serverport")]
        public string ServerPort { get; set; } = string.Empty;

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; } = string.Empty;

        [JsonPropertyName("keepalive")]
        public string KeepAlive { get; set; } = string.Empty;

        [JsonPropertyName("servers")]
        public Dictionary<string, ServerDetail> Servers { get; set; } = new Dictionary<string, ServerDetail>();
    }

    public class TunnelAddressDetail
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("selected")]
        public int Selected { get; set; }
    }

    public class ServerDetail
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("selected")]
        public int Selected { get; set; }
    }
}
