namespace VaultSecretsTool.Models
{
    record RequestUris
    {
        public string Login { get; init; }
        public string ApiOperation { get; init; }
    }
}