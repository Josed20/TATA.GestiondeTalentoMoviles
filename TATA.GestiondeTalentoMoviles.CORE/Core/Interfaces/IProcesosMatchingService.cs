using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IProcesosMatchingService
    {
        Task<ProcesosMatchingViewDto> CreateAsync(ProcesosMatchingCreateDto dto);
        Task<IEnumerable<ProcesosMatchingViewDto>> GetAllAsync();
        Task<ProcesosMatchingViewDto> GetByIdAsync(string id);
        Task<ProcesosMatchingViewDto> UpdateAsync(string id, ProcesosMatchingUpdateDto dto);
        Task DeleteAsync(string id);
    }
}
