using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces // <--- Asegúrate que el namespace sea correcto
{
    public interface IEvaluacionRepository
    {
        // Métodos de la clase EvaluacionRepository
        Task<Evaluacion> CreateAsync(Evaluacion evaluacion);
        Task<IEnumerable<Evaluacion>> GetAllAsync();
        Task<Evaluacion?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(string id, Evaluacion evaluacion);
        Task<bool> DeleteAsync(string id);
    }
}
