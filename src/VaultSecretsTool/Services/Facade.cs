using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VaultSecretsTool.Models;

namespace VaultSecretsTool.Services
{
    class Facade
    {
        private readonly SecretsService _secretsService;
        private readonly ILogger<Facade> _logger;
        private readonly HcVaultService _hcVaultService;
        private readonly SettingsHelper _settingsHelper;
        
        public Facade(ILogger<Facade> logger, HcVaultService hcVaultService, SecretsService secretsService, SettingsHelper settingsHelper)
        {
            _logger = logger;
            _hcVaultService = hcVaultService;
            _secretsService = secretsService;
            _settingsHelper = settingsHelper;
        }
        
        public async Task<Secrets> ReadSecretsAsync()
        {
            await _hcVaultService.LoginAsync();
            return await _hcVaultService.KvGetAsync();
        }

        public async Task WriteSecretsAsync()
        {
            await _hcVaultService.LoginAsync();
            var secrets = await _secretsService.LoadSecretsAsync();
            _logger.LogInformation($"Uploading secrets to the HC Vault: {secrets}");
            await _hcVaultService.KvPostAsync(secrets);

            _settingsHelper.SaveAppSetting();
        }
    }
}