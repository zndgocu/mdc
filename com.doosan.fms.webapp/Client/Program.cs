using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.IdentityData.Provider;
using com.doosan.fms.webapp.Client.SharedDatas.SingletonData.LanguageData.Provider;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using com.doosan.fms.webapp.Client.AppSetting;

namespace com.doosan.fms.webapp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddMudServices();

            builder.Services.AddSingleton(async p =>
            {
                var httpClient = p.GetRequiredService<HttpClient>();
                return await httpClient.GetFromJsonAsync<Settings>("settings.json")
                    .ConfigureAwait(false);
            });

            builder.Services.AddSingleton<CustomUserProvider>();
            builder.Services.AddSingleton<CustomLangProvider>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
