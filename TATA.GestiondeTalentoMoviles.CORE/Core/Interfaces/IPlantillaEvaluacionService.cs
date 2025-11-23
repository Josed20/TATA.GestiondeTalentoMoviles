using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IPlantillaEvaluacionService
    {
        Task<IEnumerable<PlantillaEvaluacion>> GetAllAsync();
        Task<PlantillaEvaluacion?> GetByIdAsync(string id);
        Task<PlantillaEvaluacion> CreateAsync(PlantillaEvaluacion plantilla);
        Task<bool> UpdateAsync(string id, PlantillaEvaluacion plantilla);
        Task<bool> DeleteAsync(string id);
    }
}
