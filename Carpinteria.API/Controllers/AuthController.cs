using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Auth;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(AppDbContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Validar que el DTO cumpla con las reglas (campos requeridos, email válido, etc.)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar si ya existe el email
            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });

            int rolIdAsignado = 2;

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolesId = rolIdAsignado
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado correctamente como Carpintero/Diseñador." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 2. Validar que el DTO de login no llegue con campos vacíos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var token = GenerarJwtToken(usuario);
            return Ok(new { token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (usuario == null)
            {
                return Ok(new { mensaje = "Si el correo existe, se ha generado un código de recuperación." });
            }

            var random = new Random();
            string codigoOtp = random.Next(100000, 999999).ToString();

            usuario.CodigoRecuperacion = codigoOtp;
            usuario.ExpiracionCodigo = DateTime.UtcNow.AddMinutes(10);

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.EnviarCorreoOtpAsync(usuario.Email, codigoOtp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al enviar el correo de recuperación.", detalle = ex.Message });
            }

            return Ok(new { mensaje = "Código de recuperación generado y enviado correctamente." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (usuario == null)
            {
                return BadRequest(new { mensaje = "Solicitud inválida." });
            }

            if (string.IsNullOrEmpty(usuario.CodigoRecuperacion) ||
                usuario.CodigoRecuperacion != dto.CodigoOtp ||
                usuario.ExpiracionCodigo < DateTime.UtcNow)
            {
                return BadRequest(new { mensaje = "El código es incorrecto o ha expirado." });
            }

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaPassword);
            usuario.CodigoRecuperacion = null;
            usuario.ExpiracionCodigo = null;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contraseña actualizada con éxito." });
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            string secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("La clave JWT no está configurada.");
            var key = Encoding.UTF8.GetBytes(secretKey);

            string nombreRoles = usuario.Roles?.NombreRoles ?? "Carpintero";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role, nombreRoles)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}