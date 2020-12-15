namespace VaultSecretsTool.Models
{
    record LoginCredentials
    {
        public string Username { get; init; }
        public string Password { get; init; }
    }
}