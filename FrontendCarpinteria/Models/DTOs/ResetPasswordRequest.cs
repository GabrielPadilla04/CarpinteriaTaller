using System;
using System.Collections.Generic;
using System.Text;

namespace FrontendCarpinteria.Models.DTOs
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string CodigoOtp { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
    }
}