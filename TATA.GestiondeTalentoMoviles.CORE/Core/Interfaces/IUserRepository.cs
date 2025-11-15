using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<User?> GetByIdAsync(string id);

        /// <summary>
        /// Obtiene un usuario por su refresh token
        /// </summary>
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        Task CreateAsync(User user);

        /// <summary>
        /// Actualiza un usuario existente (para guardar refresh token)
        /// </summary>
        Task UpdateAsync(User user);
    }
}
