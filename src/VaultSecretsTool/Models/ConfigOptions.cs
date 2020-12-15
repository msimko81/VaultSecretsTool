using System.Collections.Generic;

namespace VaultSecretsTool.Models
{
    record ConfigOptions
    {
        public RequestUris RequestUris { get; init; }
        public IDictionary<string, EnvironmentPaths> Paths { get; init; }
        public LoginCredentials Credentials { get; init; }
    }
}