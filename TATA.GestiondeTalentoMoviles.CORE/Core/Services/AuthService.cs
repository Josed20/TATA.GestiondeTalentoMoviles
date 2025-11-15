using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using BCrypt.Net;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            // 1. Verificar si el email ya existe
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("El email ya está registrado");
            }

            // 2. Hashear la contraseña con BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. Crear el nuevo usuario
            var newUser = new User
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email.ToLower(),
                Password = hashedPassword,
                Estado = 1, // Activo por defecto
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Roles = new List<string>() // Por defecto sin roles, se pueden asignar después
            };

            // 4. Guardar en la base de datos
            await _userRepository.CreateAsync(newUser);

            // 5. Generar tokens
            var token = GenerateJwtToken(newUser);
            var refreshToken = GenerateRefreshToken();

            // 6. Guardar el refresh token en el usuario
            newUser.RefreshToken = refreshToken;
            newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 días de validez
            await _userRepository.UpdateAsync(newUser);

            // 7. Devolver la respuesta
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpires = DateTime.UtcNow.AddHours(1),
                User = new UserResponseDto
                {
                    Id = newUser.Id!,
                    NombreCompleto = $"{newUser.Nombre} {newUser.Apellido}",
                    Email = newUser.Email,
                    Roles = newUser.Roles
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // 1. Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(dto.Email.ToLower());
            if (user == null)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas");
            }

            // 2. Verificar el estado del usuario
            if (user.Estado != 1)
            {
                throw new UnauthorizedAccessException("Usuario inactivo");
            }

            // 3. Verificar la contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas");
            }

            // 4. Generar tokens
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // 5. Guardar el refresh token en el usuario
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            // 6. Devolver la respuesta
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpires = DateTime.UtcNow.AddHours(1),
                User = new UserResponseDto
                {
                    Id = user.Id!,
                    NombreCompleto = $"{user.Nombre} {user.Apellido}",
                    Email = user.Email,
                    Roles = user.Roles
                }
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // 1. Buscar un usuario con ese refresh token
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token inválido o expirado");
            }

            // 2. Generar nuevos tokens
            var newToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // 3. Actualizar el refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            // 4. Devolver la respuesta
            return new AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpires = DateTime.UtcNow.AddHours(1),
                User = new UserResponseDto
                {
                    Id = user.Id!,
                    NombreCompleto = $"{user.Nombre} {user.Apellido}",
                    Email = user.Email,
                    Roles = user.Roles
                }
            };
        }

        #region Private Methods

        /// <summary>
        /// Genera un JWT token para el usuario
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer no configurado");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience no configurado");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims del usuario
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("nombre", user.Nombre),
                new Claim("apellido", user.Apellido)
            };

            // Agregar roles como claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token válido por 1 hora
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Genera un refresh token aleatorio
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        #endregion
    }
}
