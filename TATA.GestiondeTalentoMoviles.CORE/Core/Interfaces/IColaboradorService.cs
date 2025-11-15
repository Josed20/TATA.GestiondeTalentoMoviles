using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IColaboradorService
    {
        Task<IEnumerable<ColaboradorReadDto>> GetAllAsync();
        Task<ColaboradorReadDto?> GetByIdAsync(string id);
        Task<ColaboradorReadDto> CreateAsync(ColaboradorCreateDto createDto);
        Task<ColaboradorReadDto?> UpdateAsync(string id, ColaboradorUpdateDto updateDto);
        Task<bool> DeleteAsync(string id);
    }
}