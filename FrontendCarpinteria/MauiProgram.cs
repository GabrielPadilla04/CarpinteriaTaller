using FrontendCarpinteria.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System.Net.Http;

namespace FrontendCarpinteria
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Servicios de MudBlazor
            builder.Services.AddMudServices();

            // Interceptor de JWT y Servicio de Autenticación
            builder.Services.AddTransient<JwtTokenHandler>();
            builder.Services.AddScoped<AuthService>();

            // Cliente HTTP configurado con la URL base de producción en MonsterASP
            builder.Services.AddHttpClient("CarpinteriaAPI", client =>
            {
                client.BaseAddress = new Uri("https://carpinteria-api.runasp.net/");
            })
            .AddHttpMessageHandler<JwtTokenHandler>();

            // Esto asegura que cualquier HttpClient inyectado use la configuración de "CarpinteriaAPI"
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CarpinteriaAPI"));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}