using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _repository;

        public SkillService(ISkillRepository repository)
        {
            _repository = repository;
        }

        public async Task<SkillReadDto> CreateAsync(SkillCreateDto createDto)
        {
            var skill = new Skill
            {
                Nombre = createDto.Nombre,
                Tipo = createDto.Tipo
            };

            var nuevaSkill = await _repository.CreateAsync(skill);

            return MapToReadDto(nuevaSkill);
        }

        public async Task<IEnumerable<SkillReadDto>> GetAllAsync()
        {
            var skills = await _repository.GetAllAsync();
            return skills.Select(MapToReadDto).ToList();
        }

        public async Task<SkillReadDto?> GetByIdAsync(string id)
        {
            var skill = await _repository.GetByIdAsync(id);
            if (skill == null) return null;

            return MapToReadDto(skill);
        }

        public async Task<SkillReadDto?> UpdateAsync(string id, SkillUpdateDto updateDto)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return null;

            var skillActualizada = new Skill
            {
                Id = id,
                Nombre = updateDto.Nombre,
                Tipo = updateDto.Tipo
            };

            var ok = await _repository.UpdateAsync(id, skillActualizada);
            if (!ok) return null;

            return MapToReadDto(skillActualizada);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // Aquí hacemos borrado físico normal
            return await _repository.DeleteAsync(id);
        }

        private static SkillReadDto MapToReadDto(Skill s)
        {
            return new SkillReadDto
            {
                Id = s.Id!,
                Nombre = s.Nombre,
                Tipo = s.Tipo
            };
        }
    }
}
