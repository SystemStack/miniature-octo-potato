using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;

namespace ChatModule
{
    static internal class AccessTokens
    {
        private static IConfiguration Config { get; }
        public static string Endpoint { get; set; }
        public static Uri UriEndpoint { get; private set; }
        public static string ChatKey { get; private set; }
        public static string SecondaryChatKey { get; private set; }
        public static string ConnectionString
            => string.Format("endpoint={0};accesskey={1}", Endpoint, ChatKey);
        public static string SecondaryConnectionString
            => string.Format("endpoint={0};accesskey={1}", Endpoint, SecondaryChatKey);

        static AccessTokens()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false);
            Config = configBuilder.Build();
            var chatTokens = Config.GetSection("ChatTokens");
            var keyVaultTokens = Config.GetSection("KeyVaultTokens");
            if (chatTokens.Exists())
            {
                ChatKey = chatTokens["Key"].Trim();
                SecondaryChatKey = chatTokens["SecondaryKey"].Trim() ?? "";
                Endpoint = chatTokens["Endpoint"].Trim();
            }
            else if (keyVaultTokens.Exists())
            {
                var options = new SecretClientOptions() {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = Azure.Core.RetryMode.Exponential
                    }
                };
                var client = new SecretClient(new Uri(keyVaultTokens["Endpoint"].Trim()), new DefaultAzureCredential(), options);
                Endpoint = client.GetSecret("Endpoint").Value.Value;
                ChatKey = client.GetSecret("Key").Value.Value;
                SecondaryChatKey = client.GetSecret("SecondaryKey").Value.Value;
            }
            else
            {
                throw new Exception("appsettings.json does not have valid Chat Tokens or Azure Key Vault tokens, refer to appsettings.template.json");
            }

            UriEndpoint = new Uri(Endpoint);
        }
    }
}