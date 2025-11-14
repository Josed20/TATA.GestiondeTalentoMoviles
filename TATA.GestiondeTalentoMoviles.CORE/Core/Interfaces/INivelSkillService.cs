using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface INivelSkillService
    {
        Task<NivelSkillReadDto> CreateAsync(NivelSkillCreateDto createDto);
        Task<IEnumerable<NivelSkillReadDto>> GetAllAsync();
        Task<NivelSkillReadDto?> GetByIdAsync(string id);
        Task<NivelSkillReadDto?> UpdateAsync(string id, NivelSkillUpdateDto updateDto);
        Task<bool> DeleteAsync(string id);
    }
}