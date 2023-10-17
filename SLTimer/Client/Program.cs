using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Radzen;
using SLTimer;
using System.Globalization;

namespace SLTimer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            #region Language
            //builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            builder.Services.AddLocalization();
            #endregion

            #region Radzen
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();
            #endregion

            var jsInterop = builder.Build().Services.GetRequiredService<IJSRuntime>();
            var appLanguage = await jsInterop.InvokeAsync<string>("appCulture.get");
            if (appLanguage != null)
            {
                CultureInfo cultureInfo = new CultureInfo(appLanguage);
                CultureInfo.CurrentUICulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }
            else
            {
                CultureInfo.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("pl-PL");
                await jsInterop.InvokeVoidAsync("appCulture.set", CultureInfo.CurrentCulture.Name);
                CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("pl-PL");
                CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("pl-PL");
                CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("pl-PL");
            }

            var app = builder.Build();
            await app.RunAsync();
        }
    }
}