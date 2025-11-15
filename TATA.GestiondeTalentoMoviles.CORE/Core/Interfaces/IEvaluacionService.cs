using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IEvaluacionService
    {
        Task<EvaluacionReadDto> CreateAsync(EvaluacionCreateDto createDto);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<EvaluacionReadDto>> GetAllAsync();
        Task<EvaluacionReadDto?> GetByIdAsync(string id);
        Task<EvaluacionReadDto?> UpdateAsync(string id, EvaluacionCreateDto updateDto);
    }
}