using AgariTaku.Client.HubClients;
using AgariTaku.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgariTaku.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<GameStateService>();
            builder.Services.AddSingleton<AgariTaku.Shared.Common.IConfiguration, AgariTaku.Shared.Common.Configuration>();
            builder.Services.AddSingleton<IGameHubClientFactory, GameHubClientFactory>();

            await builder.Build().RunAsync();
        }
    }
}
