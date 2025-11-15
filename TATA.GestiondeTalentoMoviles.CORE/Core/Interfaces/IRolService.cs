using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<RolReadDto>> GetAllAsync();
        Task<RolReadDto?> GetByIdAsync(string id);
        Task<RolReadDto?> GetByNombreAsync(string nombre);
        Task<RolReadDto> CreateAsync(RolCreateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<RolReadDto?> UpdateAsync(string id, RolCreateDto dto);
    }
}