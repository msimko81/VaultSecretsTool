namespace VaultSecretsTool.Models
{
    record EnvironmentPaths
    {
        public string BaseUrl { get; init; }
        public string CosSecretsPath { get; init; }
    }
}