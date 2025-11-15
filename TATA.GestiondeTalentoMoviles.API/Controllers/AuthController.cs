using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Usuario registrado exitosamente",
                    data = response
                });
            }
            catch (InvalidOperationException ex)
            {
                // Email ya existe
                return Conflict(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al registrar usuario",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Autentica un usuario y devuelve los tokens de acceso
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Login exitoso",
                    data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Credenciales incorrectas o usuario inactivo
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al autenticar usuario",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Refresca el token de acceso usando el refresh token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(new
                {
                    success = true,
                    message = "Token refrescado exitosamente",
                    data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Refresh token inválido o expirado
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (NotImplementedException ex)
            {
                return StatusCode(501, new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al refrescar token",
                    error = ex.Message
                });
            }
        }
    }
}
