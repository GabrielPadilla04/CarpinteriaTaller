using System;
using System.Collections.Generic;
using System.Text;
using FrontendCarpinteria.DTOs;
using FrontendCarpinteria.Models;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace FrontendCarpinteria.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // Variable en memoria para acceso ultrarrápido sin demoras de escritura en disco
        private static string? _tokenActual;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginData = new LoginRequest { Email = email, Password = password };

            // Apunta al endpoint real api/Auth/login de tu AuthController
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (resultado != null && !string.IsNullOrEmpty(resultado.Token))
                {
                    // Guardamos primero en memoria para disponibilidad inmediata
                    _tokenActual = resultado.Token;

                    // Intentamos guardar en SecureStorage, con respaldo en Preferences por compatibilidad de plataforma
                    try
                    {
                        await SecureStorage.Default.SetAsync("auth_token", resultado.Token);
                    }
                    catch
                    {
                        Preferences.Default.Set("auth_token_fallback", resultado.Token);
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> RegisterAsync(RegisterRequest model)
        {
            // Apunta al endpoint de registro del backend
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var model = new ForgotPasswordRequest { Email = email };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/forgot-password", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string email, string codigoOtp, string nuevaPassword)
        {
            var model = new ResetPasswordRequest
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
                SecureStorage.Default.Remove("auth_token");
            }
            catch { }

            Preferences.Default.Remove("auth_token_fallback");
            await Task.CompletedTask;
        }

        public async Task<string?> ObtenerTokenAsync()
        {
            // Si ya lo tenemos en memoria, lo devolvemos al instante
            if (!string.IsNullOrEmpty(_tokenActual))
            {
                return _tokenActual;
            }

            // Intentamos leer del almacenamiento seguro, y si no, del respaldo local
            try
            {
                _tokenActual = await SecureStorage.Default.GetAsync("auth_token");
            }
            catch
            {
                _tokenActual = null;
            }

            if (string.IsNullOrEmpty(_tokenActual))
            {
                _tokenActual = Preferences.Default.Get("auth_token_fallback", string.Empty);
            }

            return _tokenActual;
        }
    }
}