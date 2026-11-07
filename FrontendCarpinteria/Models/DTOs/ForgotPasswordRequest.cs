using System;
using System.Collections.Generic;
using System.Text;

namespace FrontendCarpinteria.Models.DTOs
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}