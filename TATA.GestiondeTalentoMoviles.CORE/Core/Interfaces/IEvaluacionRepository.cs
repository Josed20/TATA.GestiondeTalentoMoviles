using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IEvaluacionRepository
    {
        Task<IEnumerable<Evaluacion>> GetAllAsync();
        Task<Evaluacion?> GetByIdAsync(string id);
        Task<IEnumerable<Evaluacion>> GetByColaboradorAsync(string colaboradorId);
        Task AddAsync(Evaluacion evaluacion);
        Task UpdateAsync(Evaluacion evaluacion);
        Task DeleteAsync(string id);
    }
}
