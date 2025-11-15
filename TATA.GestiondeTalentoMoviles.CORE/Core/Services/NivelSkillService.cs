using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class NivelSkillService : INivelSkillService
    {
        private readonly INivelSkillRepository _repository;

        public NivelSkillService(INivelSkillRepository repository)
        {
            _repository = repository;
        }

        public async Task<NivelSkillReadDto> CreateAsync(NivelSkillCreateDto createDto)
        {
            var nivel = new NivelSkill
            {
                Codigo = createDto.Codigo,
                Nombre = createDto.Nombre
                // Id se deja null, MongoDB lo generará automáticamente
            };

            try
            {
                var nuevoNivel = await _repository.CreateAsync(nivel);
                return MapToReadDto(nuevoNivel);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException($"Ya existe un nivel de skill con código {createDto.Codigo}", ex);
            }
        }

        public async Task<IEnumerable<NivelSkillReadDto>> GetAllAsync()
        {
            var niveles = await _repository.GetAllAsync();
            return niveles.Select(MapToReadDto).ToList();
        }

        public async Task<NivelSkillReadDto?> GetByIdAsync(string id)
        {
            var nivel = await _repository.GetByIdAsync(id);
            if (nivel == null) return null;

            return MapToReadDto(nivel);
        }

        public async Task<NivelSkillReadDto?> UpdateAsync(string id, NivelSkillUpdateDto updateDto)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return null;

            var nivelActualizado = new NivelSkill
            {
                Id = id,
                Codigo = updateDto.Codigo,
                Nombre = updateDto.Nombre
            };

            var ok = await _repository.UpdateAsync(id, nivelActualizado);
            if (!ok) return null;

            return MapToReadDto(nivelActualizado);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return false;

            return await _repository.DeleteAsync(id);
        }

        private static NivelSkillReadDto MapToReadDto(NivelSkill n)
        {
            return new NivelSkillReadDto
            {
                Id = n.Id!,
                Codigo = n.Codigo,
                Nombre = n.Nombre
            };
        }
    }
}
