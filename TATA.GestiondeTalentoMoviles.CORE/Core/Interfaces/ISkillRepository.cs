using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ISkillRepository
    {
        Task<Skill> CreateAsync(Skill skill);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<Skill>> GetAllAsync();
        Task<Skill?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(string id, Skill skill);
    }
}