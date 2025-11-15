using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IVacanteService
    {
        Task<IEnumerable<VacanteReadDto>> GetAllAsync();
        Task<VacanteReadDto?> GetByIdAsync(string id);
        Task<VacanteReadDto> CreateAsync(VacanteCreateDto createDto);
        Task<VacanteReadDto?> UpdateAsync(string id, VacanteCreateDto updateDto);
        Task<bool> DeleteAsync(string id);
    }
}
