using FrontendCarpinteriaWeb;
using FrontendCarpinteriaWeb.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// Registrar el manejador de tokens
builder.Services.AddTransient<JwtTokenHandler>();

// Registrar AuthService
builder.Services.AddScoped<AuthService>();

// Configurar el HttpClient para que pase obligatoriamente por el JwtTokenHandler
builder.Services.AddHttpClient("ApiExterna", client =>
{
    client.BaseAddress = new Uri("https://carpinteria-api.runasp.net/");
})
.AddHttpMessageHandler<JwtTokenHandler>();

// Hacer que el HttpClient por defecto inyectado con @inject use la configuración con el handler
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiExterna"));

await builder.Build().RunAsync();