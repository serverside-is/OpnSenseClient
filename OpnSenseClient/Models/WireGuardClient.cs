using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpnSenseClient.Models
{
    public class WireGuardClient
    {
        // This one is not mapped to JSON, because the JSON format used stinks and provides this value outside of the client data object representation
        [JsonIgnore]
        public string ServerId { get; set; } = string.Empty;

        [JsonIgnore]
        public string ServerUuid { get; set; } = string.Empty;

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        [JsonPropertyName("enabled")]
        public string Enabled { get; set; } = "0";

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("pubkey")]
        public string PubKey { get; set; } = string.Empty;

        [JsonPropertyName("tunneladdress")]
        public string TunnelAddress { get; set; } = string.Empty;

        [JsonPropertyName("serveraddress")]
        public string ServerAddress { get; set; } = string.Empty;

        [JsonPropertyName("serverport")]
        public string ServerPort { get; set; } = string.Empty;

        [JsonPropertyName("servers")]
        public string WireGuardServerUuid { get; set; } = string.Empty;
    }

    public class WireGuardServer
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
