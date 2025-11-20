using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories
{
    public interface IProcesosMatchingRepository
    {
        Task<ProcesosMatching> GetByIdAsync(string id);
        Task<IEnumerable<ProcesosMatching>> GetAllAsync();
        Task<IEnumerable<ProcesosMatching>> GetByVacanteIdAsync(string vacanteId);
        Task<IEnumerable<ProcesosMatching>> GetByFechaCreacionAsync(System.DateTime fechaCreacion);
        Task CreateAsync(ProcesosMatching proceso);
        Task UpdateAsync(string id, ProcesosMatching procesoIn);
        Task DeleteAsync(string id);
    }
}
