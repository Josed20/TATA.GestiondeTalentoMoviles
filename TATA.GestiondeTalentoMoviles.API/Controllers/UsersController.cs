using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.Constants;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = AppRoles.ADMIN)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene todos los usuarios (Solo ADMIN)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { success = true, data = users });
        }

        /// <summary>
        /// Obtiene un usuario por ID (Permite acceso anónimo)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "Usuario no encontrado" });

            return Ok(new { success = true, data = user });
        }

        /// <summary>
        /// Crea un nuevo usuario (Solo ADMIN)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Error de validación",
                        errors = ModelState
                    });
                }

                var newUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, new { success = true, data = newUser });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente (Solo ADMIN)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Error de validación",
                        errors = ModelState
                    });
                }

                var updatedUser = await _userService.UpdateUserAsync(id, dto);
                return Ok(new { success = true, message = "Usuario actualizado exitosamente", data = updatedUser });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un usuario (Solo ADMIN)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { success = true, message = "Usuario eliminado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Resetea la contraseña de un usuario (Solo ADMIN)
        /// </summary>
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] UserResetPasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Error de validación",
                        errors = ModelState
                    });
                }

                await _userService.ResetPasswordAsync(id, dto);
                return Ok(new { success = true, message = "Contraseña reseteada exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Desbloquea un usuario bloqueado por intentos fallidos (Solo ADMIN)
        /// </summary>
        [HttpPost("{id}/unblock")]
        public async Task<IActionResult> Unblock(string id)
        {
            try
            {
                await _userService.UnblockUserAsync(id);
                return Ok(new { success = true, message = "Usuario desbloqueado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}