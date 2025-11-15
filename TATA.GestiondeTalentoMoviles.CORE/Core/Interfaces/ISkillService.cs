using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ISkillService
    {
        Task<SkillReadDto> CreateAsync(SkillCreateDto createDto);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<SkillReadDto>> GetAllAsync();
        Task<SkillReadDto?> GetByIdAsync(string id);
        Task<SkillReadDto?> UpdateAsync(string id, SkillUpdateDto updateDto);
    }
}