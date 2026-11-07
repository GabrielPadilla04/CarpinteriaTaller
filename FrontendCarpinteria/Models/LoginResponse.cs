using System;
using System.Collections.Generic;
using System.Text;

namespace FrontendCarpinteria.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}
