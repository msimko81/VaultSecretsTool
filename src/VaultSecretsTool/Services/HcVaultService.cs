using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VaultSecretsTool.Models;

namespace VaultSecretsTool.Services
{
    class HcVaultService
    {
        private const string Environment = "Environment";

        private readonly ILogger<HcVaultService> _logger;

        private readonly RequestUris _requestUris;
        private readonly LoginCredentials _credentials;
        private readonly string _cosSecretPath;

        private readonly HttpClient _httpClient;

        public HcVaultService(IOptions<ConfigOptions> options, IConfiguration configuration,
            ILogger<HcVaultService> logger,
            HttpClient httpClient, IUserInputService userInputService)
        {
            _logger = logger;

            var config = options.Value;
            _requestUris = config.RequestUris;

            var environment = userInputService.PromptUserForInput("Choose the used HC Vault environment ({0}):",
                configuration[Environment], new []{"Development", "Staging", "Production"});
            configuration[Environment] = environment;
            
            var paths = config.Paths[environment];
            _cosSecretPath = paths.CosSecretsPath;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(paths.BaseUrl);

            _credentials = config.Credentials;
        }

        /// <summary>
        /// Login to HC Vault using <a href="https://www.vaultproject.io/api-docs/auth/ldap#login-with-ldap-user">login-with-ldap-user</a>.
        /// </summary>
        public async Task LoginAsync()
        {
            _logger.LogInformation($"Logging to the HC Vault for {_credentials.Username}");
            var httpResp = await _httpClient.PostAsJsonAsync(string.Format(_requestUris.Login, _credentials.Username),
                _credentials with {Username = null});
            var loginResponse = await httpResp.Content.ReadFromJsonAsync<LoginResponse>();

            _logger.LogInformation($"Client token: {loginResponse?.Auth.ClientToken}");

            _httpClient.DefaultRequestHeaders.Remove("X-Vault-Token");
            _httpClient.DefaultRequestHeaders.Add("X-Vault-Token", loginResponse?.Auth.ClientToken);
        }

        /// <summary>
        /// Write a secret to HC Vault using <a href="https://www.vaultproject.io/api-docs#api-operations">api-operations</a>.
        /// </summary>
        public async Task KvPostAsync(Secrets secrets)
        {
            _logger.LogInformation($"Sending secrets to the HC Vault with the path {_cosSecretPath}");

            var httpResp = await _httpClient.PostAsJsonAsync(string.Format(_requestUris.ApiOperation, _cosSecretPath),
                secrets);

            if (httpResp.StatusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError(
                    $"Error while writing secrets to the HC Vault. Response status code: {httpResp.StatusCode}");
            }
        }

        /// <summary>
        /// Get a secret from HC Vault using <a href="https://www.vaultproject.io/api-docs#api-operations">api-operations</a>.
        /// </summary>
        public async Task<Secrets> KvGetAsync()
        {
            _logger.LogInformation($"Reading secrets from the HC Vault with the path {_cosSecretPath}");

            var response =
                await _httpClient.GetFromJsonAsync<KvGetResponse>(string.Format(_requestUris.ApiOperation,
                    _cosSecretPath));

            return response?.Data;
        }
    }
}