using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IVacanteRepository
    {
        Task<IEnumerable<Vacante>> GetAllAsync();
        Task<Vacante?> GetByIdAsync(string id);
        Task<Vacante> CreateAsync(Vacante vacante);
        Task<Vacante?> UpdateAsync(string id, Vacante vacante);
        Task<bool> DeleteAsync(string id);
    }
}
