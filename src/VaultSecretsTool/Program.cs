using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;
using VaultSecretsTool.Extensions.Configuration;
using VaultSecretsTool.Models;
using VaultSecretsTool.Services;

namespace VaultSecretsTool
{
    static class Program
    {
        private static async Task Main(string[] args)
        {
            InitializeLogger();

            try
            {
                using var host = CreateHostBuilder().Build();
                var service = host.Services.GetService<Facade>();
                await service.WriteSecretsAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static IHostBuilder AddAppConfiguration(this IHostBuilder builder, IUserInputService userInputService) =>
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.Local.json", true)
                    .AddJsonFile("appsettings.json")
                    .AddUserInputConfigurationSource(userInputService);
            });

        private static IHostBuilder AddServices(this IHostBuilder builder, IUserInputService userInputService) =>
            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, true));

                services.Configure<ConfigOptions>(hostContext.Configuration.GetSection("HcVault"));

                services.AddSingleton<HcVaultService>();
                services.AddSingleton<Facade>();
                services.AddSingleton<SecretsService>();
                services.AddSingleton<SettingsHelper>();
                services.AddSingleton(userInputService);

                services.AddHttpClient<HcVaultService>()
                    .AddPolicyHandler(HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .RetryAsync(3))
                    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(10));
            });

        private static IHostBuilder CreateHostBuilder()
        {
            var userInputService = new UserInputService();
            return new HostBuilder()
                .AddAppConfiguration(userInputService)
                .AddServices(userInputService);
        }
    }
}