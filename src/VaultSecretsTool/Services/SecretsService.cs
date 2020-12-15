using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VaultSecretsTool.Models;

namespace VaultSecretsTool.Services
{
    /// <summary>
    /// Service for loading secrets from a json file.
    /// </summary>
    class SecretsService
    {
        private const string SecretsLocalFilePath = "SecretsLocalFilePath";

        private readonly ILogger<SecretsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserInputService _userInputService;


        public SecretsService(ILogger<SecretsService> logger, IConfiguration configuration, IUserInputService
            userInputService)
        {
            _logger = logger;
            _configuration = configuration;
            _userInputService = userInputService;
        }

        /// <summary>
        /// Loads secrets from configured filename (configuration property <see cref="SecretsLocalFilePath"/>), if defined.
        /// In case the filename is not defined, list all json files from the working directory
        /// and let the user choose the one to load.
        /// </summary>
        /// <returns>Loaded secrets.</returns>
        public async Task<Secrets> LoadSecretsAsync()
        {
            var secretsLocalFilePath = ChooseFileToLoad();
            _configuration[SecretsLocalFilePath] = secretsLocalFilePath;
            return await LoadSecretsFromFileAsync(secretsLocalFilePath);
        }

        /// <summary>
        /// List all json files (excluding .NET project files) from a working directory and its subdirectories.
        /// Ask user to either choose one listed file by index or to insert full custom file path.
        /// </summary>
        /// <returns>JSON file name to load secrets from.</returns>
        private string ChooseFileToLoad()
        {
            var filenames =
                Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.json", SearchOption.AllDirectories)
                    .Where(f =>
                    {
                        var filename = new FileInfo(f).Name;
                        return !(filename.StartsWith("appsettings") || filename.StartsWith("VaultSecretsTool") ||
                                 filename.StartsWith("project"));
                    });

            var secretsLocalFilePath = _configuration[SecretsLocalFilePath];
            return _userInputService.PromptUserForInput(
                "Choose file to load secrets from or insert custom file name ({0}):", secretsLocalFilePath, filenames);
        }

        /// <summary>
        /// Load secrets from given json file.
        /// Supported file shall contain one top level object with arbitrary number of string properties.
        /// The file is mapped to a <i>Dictionary&lt;string,string&gt;</i>.
        /// </summary>
        /// <param name="filePath">Full file path of json file to load secrets from.</param>
        /// <returns>Loaded secrets.</returns>
        private async Task<Secrets> LoadSecretsFromFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation($"Loading secrets from {filePath}");

                await using var stream = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<Secrets>(stream);
            }
            catch (Exception e)
            {
                throw new Exception($"Error while loading secrets from {filePath}", e);
            }
        }
    }
}