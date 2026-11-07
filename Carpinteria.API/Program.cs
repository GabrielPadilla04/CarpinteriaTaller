using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de datos (Lee automáticamente de launchSettings.json o variables de entorno)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30))));

// 2. Autenticación JWT (Leyendo de forma segura desde la configuración)
var jwtSettings = builder.Configuration.GetSection("Jwt");
string secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("La clave JWT no está configurada en el entorno.");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true
    };
});

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Permisivo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Inyección de Dependencias
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<AmbienteService>();
builder.Services.AddScoped<MedicionService>();
builder.Services.AddScoped<BitacoraService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<NotificacionService>();
builder.Services.AddScoped<VisitaEntregaService>();
builder.Services.AddScoped<EstadoProyectoService>();
builder.Services.AddScoped<ProyectoMaterialService>();
builder.Services.AddScoped<CategoriaMaterialService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<CompraService>();
builder.Services.AddScoped<EstadoTallerService>();
// builder.Services.AddScoped<ImagenService>(); // Descomentar solo si usas carga de imágenes local

// 5. Controladores y OpenAPI NATIVO de .NET
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 6. Pipeline HTTP
app.MapOpenApi();

// Configuración correcta para Scalar con OpenAPI nativo de .NET
app.MapScalarApiReference(options =>
{
    options.WithTitle("Carpintería API")
           .WithOpenApiRoutePattern("/openapi/v1.json");
});

// Redirecciona automáticamente la raíz (/) directamente hacia la interfaz de Scalar
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Permisivo");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();