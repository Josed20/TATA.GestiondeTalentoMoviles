using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaReadDto>> GetAllAsync();
        Task<AreaReadDto?> GetByIdAsync(string id);
        Task<AreaReadDto?> GetByNombreAsync(string nombre);
        Task<AreaReadDto> CreateAsync(AreaCreateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<AreaReadDto?> UpdateAsync(string id, AreaCreateDto dto);
    }
}