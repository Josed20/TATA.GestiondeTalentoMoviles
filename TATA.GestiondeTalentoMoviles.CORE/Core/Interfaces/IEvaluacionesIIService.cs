using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IEvaluacionesIIService
    {
        Task<IEnumerable<EvaluacionesII>> GetAllAsync();
        Task<EvaluacionesII?> GetByIdAsync(string id);
        Task AddAsync(EvaluacionesII plantilla);
        Task UpdateAsync(EvaluacionesII plantilla);
        Task DeleteAsync(string id);
    }
}
