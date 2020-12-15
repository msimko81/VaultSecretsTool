namespace VaultSecretsTool.Models
{
    record LoginResponse
    {
        public Authorization Auth { get; init; }
    }
}