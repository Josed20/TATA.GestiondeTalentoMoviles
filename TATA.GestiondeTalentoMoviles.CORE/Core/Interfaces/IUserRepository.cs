using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByNombreApellidoAsync(string nombre, string apellido);
        Task<User> CreateAsync(User user);
        Task<bool> DeleteAsync(string id);
        Task<User?> UpdateAsync(string id, User user);
    }
}