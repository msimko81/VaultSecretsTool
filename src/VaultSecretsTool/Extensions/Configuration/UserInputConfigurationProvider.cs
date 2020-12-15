using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using VaultSecretsTool.Services;

namespace VaultSecretsTool.Extensions.Configuration
{
    class UserInputConfigurationProvider : ConfigurationProvider
    {
        private readonly IConfiguration _parentConfiguration;
        private readonly IUserInputService _userInputService;
        private readonly List<string> _valueKeysEligibleForUserInput;

        public UserInputConfigurationProvider(IConfiguration parentConfiguration, IUserInputService userInputService)
        {
            _parentConfiguration = parentConfiguration;
            _userInputService = userInputService;
            _valueKeysEligibleForUserInput = parentConfiguration.GetSection("ValueKeysEligibleForUserInput")
                .AsEnumerable()
                .Select(k => k.Value)
                .Where(v => v != null)
                .ToList();
        }

        public override void Load()
        {
            Data = new UserInputDictionary(_parentConfiguration, _userInputService);
        }

        public override bool TryGet(string key, out string value)
        {
            value = _parentConfiguration[key];
            return (_valueKeysEligibleForUserInput.Contains(key) && base.TryGet(key, out value)) || value != null;
        }
    }
}