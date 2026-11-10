using System;

namespace FrontendCarpinteriaWeb.DTOs.Login
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public int RolesId { get; set; }
    }
}