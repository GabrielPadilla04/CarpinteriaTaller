using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using FrontendCarpinteriaWeb.DTOs.Login;

namespace FrontendCarpinteriaWeb.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private static string? _tokenActual;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        // Cambiamos el retorno a Task<LoginResponse?> para poder evaluar el rol
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var loginData = new LoginRequest { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (resultado != null && !string.IsNullOrEmpty(resultado.Token))
                {
                    _tokenActual = resultado.Token;
                    try
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_token", resultado.Token);
                    }
                    catch { }

                    return resultado; // Retornamos el objeto completo con el RolesId
                }
            }

            return null;
        }

        public async Task<bool> RegisterAsync(RegisterRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var model = new { Email = email };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/forgot-password", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string email, string codigoOtp, string nuevaPassword)
        {
            var model = new
            {
                Email = email,
                CodigoOtp = codigoOtp,
                NuevaPassword = nuevaPassword
            };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/reset-password", model);
            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync()
        {
            _tokenActual = null;
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_token");
            }
            catch { }
        }

        public async Task<string?> ObtenerTokenAsync()
        {
            if (!string.IsNullOrEmpty(_tokenActual))
            {
                return _tokenActual;
            }

            try
            {
                _tokenActual = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_token");
            }
            catch
            {
                _tokenActual = null;
            }

            return _tokenActual;
        }
    }
}