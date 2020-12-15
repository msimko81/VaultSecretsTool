using System.Text.Json.Serialization;

namespace VaultSecretsTool.Models
{
    record Authorization
    {
        [JsonPropertyName("client_token")] public string ClientToken { get; init; }
    }
}