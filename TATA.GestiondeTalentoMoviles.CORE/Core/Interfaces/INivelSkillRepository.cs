using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface INivelSkillRepository
    {
        Task<NivelSkill> CreateAsync(NivelSkill nivelSkill);
        Task<IEnumerable<NivelSkill>> GetAllAsync();
        Task<NivelSkill?> GetByIdAsync(string id);
        Task<NivelSkill?> GetByCodigoAsync(int codigo);
        Task<bool> UpdateAsync(string id, NivelSkill nivelSkill);
        Task<bool> DeleteAsync(string id);
    }
}