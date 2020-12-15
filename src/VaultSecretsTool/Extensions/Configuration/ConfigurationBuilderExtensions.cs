using Microsoft.Extensions.Configuration;
using VaultSecretsTool.Services;

namespace VaultSecretsTool.Extensions.Configuration
{
    static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddUserInputConfigurationSource(this IConfigurationBuilder builder,
            IUserInputService userInputService)
        {
            return builder.Add(new UserInputConfigurationSource(builder, userInputService));
        }
    }
}