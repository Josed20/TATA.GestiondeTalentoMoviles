using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private const int MAX_LOGIN_ATTEMPTS = 5;
        private const int LOCKOUT_MINUTES = 15;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(AuthRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            // 1. Validar si el usuario existe
            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrecta.");

            // 2. Validar si la cuenta está bloqueada
            if (user.BloqueadoHasta.HasValue && user.BloqueadoHasta > DateTime.UtcNow)
                throw new UnauthorizedAccessException($"Cuenta bloqueada temporalmente. Intente de nuevo en {LOCKOUT_MINUTES} minutos.");

            // 3. Validar la contraseña
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                // Si falla: Incrementar intentos y bloquear si es necesario
                user.IntentosFallidos++;
                if (user.IntentosFallidos >= MAX_LOGIN_ATTEMPTS)
                {
                    user.BloqueadoHasta = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES);
                }
                await _userRepository.UpdateAsync(user.Id, user); // Guardar cambios
                throw new UnauthorizedAccessException("Usuario o contraseña incorrecta.");
            }

            // 4. ¡Éxito! Limpiar intentos y generar token
            user.IntentosFallidos = 0;
            user.BloqueadoHasta = null;
            user.UltimoAcceso = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user.Id, user); // Guardar cambios

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                RolSistema = user.RolSistema,
                ColaboradorId = user.ColaboradorId
            };
        }

        public async Task ChangePasswordAsync(string userId, UserChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            // Verificar que la contraseña actual sea correcta
            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            
            if (!isCurrentPasswordValid)
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");

            // Hashear y guardar la nueva contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            
            await _userRepository.UpdateAsync(user.Id, user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("La clave JWT (Jwt:Key) no está configurada.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tus "Claims" son la info dentro del token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RolSistema ?? string.Empty),
                // Add also 'role' and 'roles' claims to be compatible with different consumers
                new Claim("role", user.RolSistema ?? string.Empty),
                new Claim("roles", user.RolSistema ?? string.Empty),
                new Claim("uid", user.Id),
                new Claim("cid", user.ColaboradorId ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}