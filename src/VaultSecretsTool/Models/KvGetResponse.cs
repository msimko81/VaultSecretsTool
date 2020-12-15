namespace VaultSecretsTool.Models
{
    record KvGetResponse
    {
        public Secrets Data { get; init; }
    }
}