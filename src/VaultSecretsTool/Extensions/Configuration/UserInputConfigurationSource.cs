using Microsoft.Extensions.Configuration;
using VaultSecretsTool.Services;

namespace VaultSecretsTool.Extensions.Configuration
{
    class UserInputConfigurationSource : IConfigurationSource
    {
        private readonly IUserInputService _userInputService;
        private readonly IConfiguration _parentConfiguration;

        public UserInputConfigurationSource(IConfigurationBuilder builder, IUserInputService userInputService)
        {
            _userInputService = userInputService;
            _parentConfiguration = builder.Build();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new UserInputConfigurationProvider(_parentConfiguration, _userInputService);
    }
}