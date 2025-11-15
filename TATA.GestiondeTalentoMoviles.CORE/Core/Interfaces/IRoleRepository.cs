using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(string id);
        Task CreateAsync(Role role);
        Task<bool> UpdateAsync(string id, Role role);
        Task<bool> DeleteAsync(string id);
    }
}
