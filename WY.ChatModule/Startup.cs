using ChatModule.Models;
using ChatModule.Services;
using ChatModule.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatModule
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Utils.IsNotNull(AccessTokens.ConnectionString, "Connection String");
            Utils.IsNotNull(AccessTokens.ChatKey, "Chat Key");
            Utils.IsNotNull(AccessTokens.Endpoint, "Endpoint");
            Utils.IsNotNull(AccessTokens.UriEndpoint, "URI Endpoint");
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

            });

            services.AddSingleton(new Store<User>())
                    .AddSingleton(new Store<Thread>());
            services.AddScoped(typeof(UserService))
                    .AddScoped(typeof(ThreadService));

            // TODO: API Versioning
            //services.AddApiVersioning(apiOptions => {
            //    apiOptions.DefaultApiVersion = new ApiVersion(1, 0);
            //    apiOptions.AssumeDefaultVersionWhenUnspecified = true;
            //    apiOptions.ReportApiVersions = false;
            //});
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }
}
