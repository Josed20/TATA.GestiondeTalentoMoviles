using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<User?> GetByIdAsync(string id);

        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene un usuario por nombre y apellido
        /// </summary>
        Task<User?> GetByNombreApellidoAsync(string nombre, string apellido);

        /// <summary>
        /// Obtiene un usuario por su refresh token
        /// </summary>
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        Task<User?> UpdateAsync(string id, User user);

        /// <summary>
        /// Actualiza un usuario existente (para guardar refresh token)
        /// </summary>
        Task UpdateAsync(User user);

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        Task<bool> DeleteAsync(string id);
    }
}
