using ChatModule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatModule.Services;
using ChatModule.Stores;

namespace ChatModule
{
    public sealed class Startup
    {
        public Startup()
        {
            Utils.IsNotNull(AccessTokens.ConnectionString, "Connection String");
            Utils.IsNotNull(AccessTokens.ChatKey, "Chat Key");
            Utils.IsNotNull(AccessTokens.Endpoint, "Endpoint");
            Utils.IsNotNull(AccessTokens.UriEndpoint, "URI Endpoint");
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    // Threadsafe Concurrency Dictionaries for r/w
                    services
                        .AddSingleton(new Store<User>())
                        .AddSingleton(new Store<Thread>());

                    // Microsoft clients instantiated within Services
                    services.AddScoped(typeof(UserService));
                    services.AddApiVersioning(x =>
                    {
                        x.DefaultApiVersion = new ApiVersion(1, 0);
                        x.AssumeDefaultVersionWhenUnspecified = true;
                        x.ReportApiVersions = true;
                    });
                });
        }
    }
}
