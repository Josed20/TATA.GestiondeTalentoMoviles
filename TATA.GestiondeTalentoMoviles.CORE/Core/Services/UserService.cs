using TATA.GestiondeTalentoMoviles.CORE.Core.Constants;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserViewDto> CreateUserAsync(UserCreateDto userDto)
        {
            // 1. Validar que el rol sea válido
            if (!AppRoles.IsValidRole(userDto.RolSistema))
                throw new InvalidOperationException($"El rol '{userDto.RolSistema}' no es válido. Roles válidos: {string.Join(", ", AppRoles.GetAllRoles())}");

            // 2. Validar que el usuario no exista
            var existingUser = await _userRepository.GetByUsernameAsync(userDto.Username);
            if (existingUser != null)
                throw new InvalidOperationException("El nombre de usuario ya existe.");

            // 3. Hashear el password (¡NUNCA GUARDAR TEXTO PLANO!)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // 4. Crear la nueva entidad
            var newUser = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                RolSistema = userDto.RolSistema,
                ColaboradorId = userDto.ColaboradorId,
                FechaCreacion = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow,
                IntentosFallidos = 0,
                BloqueadoHasta = null
            };

            // 5. Guardar en BD
            await _userRepository.CreateAsync(newUser);

            // 6. Devolver el DTO (sin info sensible)
            return MapToViewDto(newUser);
        }

        public async Task<UserViewDto> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToViewDto(user);
        }

        public async Task<IEnumerable<UserViewDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToViewDto);
        }

        public async Task<UserViewDto> UpdateUserAsync(string id, UserUpdateDto dto)
        {
            // 1. Verificar que el usuario existe
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            // 2. Validar que el rol sea válido
            if (!AppRoles.IsValidRole(dto.RolSistema))
                throw new InvalidOperationException($"El rol '{dto.RolSistema}' no es válido. Roles válidos: {string.Join(", ", AppRoles.GetAllRoles())}");

            // 3. Actualizar campos
            user.Email = dto.Email;
            user.RolSistema = dto.RolSistema;
            user.ColaboradorId = dto.ColaboradorId;

            // 4. Guardar cambios
            await _userRepository.UpdateAsync(id, user);

            // 5. Devolver el usuario actualizado
            return MapToViewDto(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            await _userRepository.DeleteAsync(id);
        }

        public async Task ResetPasswordAsync(string id, UserResetPasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            // Hashear la nueva contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // Limpiar bloqueos por si acaso
            user.IntentosFallidos = 0;
            user.BloqueadoHasta = null;

            await _userRepository.UpdateAsync(id, user);
        }

        public async Task UnblockUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            // Resetear intentos fallidos y bloqueo
            user.IntentosFallidos = 0;
            user.BloqueadoHasta = null;

            await _userRepository.UpdateAsync(id, user);
        }

        // Helper privado para no exponer la entidad
        private UserViewDto MapToViewDto(User user)
        {
            return new UserViewDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RolSistema = user.RolSistema,
                ColaboradorId = user.ColaboradorId,
                IntentosFallidos = user.IntentosFallidos,
                BloqueadoHasta = user.BloqueadoHasta,
                UltimoAcceso = user.UltimoAcceso,
                FechaCreacion = user.FechaCreacion
            };
        }
    }
}