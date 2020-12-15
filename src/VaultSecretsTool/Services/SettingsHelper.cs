using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VaultSecretsTool.Models;

namespace VaultSecretsTool.Services
{
    class SettingsHelper
    {
        private const string SavePassword = "SavePassword";

        private readonly ILogger<SettingsHelper> _logger;
        private readonly IOptions<ConfigOptions> _configOptions;
        private readonly IConfiguration _configuration;
        private readonly IUserInputService _userInputService;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public SettingsHelper(ILogger<SettingsHelper> logger, IOptions<ConfigOptions> configOptions,
            IConfiguration configuration, IUserInputService userInputService)
        {
            _logger = logger;
            _configOptions = configOptions;
            _configuration = configuration;
            _userInputService = userInputService;
        }

        public void SaveAppSetting()
        {
            SaveAppSettings(Path.Combine(AppContext.BaseDirectory, "appsettings.Local.json"), PrepareAppSettings());
        }

        private void SaveAppSettings(string filePath, Dictionary<string, dynamic> appSettings)
        {
            var output = JsonSerializer.Serialize(appSettings, JsonOptions);
            File.WriteAllText(filePath, output);
            _logger.LogInformation($"Application settings were saved to {filePath}");
        }

        private Dictionary<string, dynamic> PrepareAppSettings()
        {
            var saveSettingsWithPassword =
                _userInputService.PromptUserForInput(
                    "Save password together with another settings? ({0})",
                    _configuration[SavePassword],
                    new[] { "True", "False" });

            var settings = _configOptions.Value;
            if (!"True".Equals(saveSettingsWithPassword))
            {
                var credWithoutPassword = settings.Credentials with {Password = null};
                settings = settings with{ Credentials = credWithoutPassword};
            }

            return new Dictionary<string, dynamic>
            {
                { "SavePassword", saveSettingsWithPassword },
                { "HcVault", settings },
                { "Environment", _configuration["Environment"] },
                { "SecretsLocalFilePath", _configuration["SecretsLocalFilePath"] }
            };
        }
    }
}