using ChatModule.Controllers;
using ChatModule.Models;
using ChatModule.Services;
using ChatModule.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatModule
{
    public sealed class Startup
    {
        // TODO: Finish configuring
        // TODO: JSON Formatters
        public void Configure(IApplicationBuilder app)
        {
            Utils.IsNotNull(AccessTokens.ConnectionString, "Connection String");
            Utils.IsNotNull(AccessTokens.ChatKey, "Chat Key");
            Utils.IsNotNull(AccessTokens.Endpoint, "Endpoint");
            Utils.IsNotNull(AccessTokens.UriEndpoint, "URI Endpoint");
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => {
                    // Threadsafe Concurrency Dictionaries for r/w
                    services
                        .AddSingleton(new Store<User>())
                        .AddSingleton(new Store<Thread>());
                    
                    // Azure Communication clients instantiated within Services
                    services.AddScoped(typeof(UserService));
                    services.AddApiVersioning(apiOptions => {
                        apiOptions.DefaultApiVersion = new ApiVersion(1, 0);
                        apiOptions.AssumeDefaultVersionWhenUnspecified = true;
                        apiOptions.ReportApiVersions = true;
                    });

                    services.AddControllers(controllerOptions => {
                        controllerOptions.AllowEmptyInputInBodyModelBinding = true;
                    });
                });
        }
    }
}
