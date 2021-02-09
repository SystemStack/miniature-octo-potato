using System;
using Microsoft.Extensions.Configuration;

namespace ChatModule
{
    static class AccessTokens
    {
        private static IConfiguration Config { get; }
        public static string ChatKey { get; private set; }
        public static string ConnectionString { get; private set; }
        public static string Endpoint { get; set; }
        public static Uri UriEndpoint { get; private set; }
        static AccessTokens()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false);
            Config = configBuilder.Build();

            var primary = Config.GetSection("ChatTokens");
            ChatKey = primary["Key"].Trim();
            Endpoint = primary["EndPoint"].Trim();
            ConnectionString = Config.GetConnectionString("PrimaryChat").Trim();
            UriEndpoint = new Uri(Endpoint);
        }
        
        // TODO: Implement
        public static void CatchPrimaryTokenFailure()
        {
            var secondary = Config.GetSection("SecondaryChatTokens");
            ChatKey = secondary["Key"].Trim() ?? ChatKey;
            Endpoint = secondary["EndPoint"].Trim() ?? Endpoint;
            ConnectionString = Config.GetConnectionString("SecondaryChat").Trim() ?? ConnectionString;
            UriEndpoint = new Uri(Endpoint);
        }
    }
}