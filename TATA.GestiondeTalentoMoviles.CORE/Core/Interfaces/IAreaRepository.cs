using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAsync();
        Task<Area?> GetByIdAsync(string id);
        Task<Area?> GetByNombreAsync(string nombre);
        Task<Area> CreateAsync(Area area);
        Task<bool> DeleteAsync(string id);
        Task<Area?> UpdateAsync(string id, Area area);
    }
}