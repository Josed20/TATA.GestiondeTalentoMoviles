using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(string id);
        Task<Rol?> GetByNombreAsync(string nombre);
        Task<Rol> CreateAsync(Rol rol);
        Task<bool> DeleteAsync(string id);
        Task<Rol?> UpdateAsync(string id, Rol rol);
    }
}