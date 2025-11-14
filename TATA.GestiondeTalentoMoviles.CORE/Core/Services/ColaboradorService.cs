using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly IColaboradorRepository _repository;

        public ColaboradorService(IColaboradorRepository repository)
        {
            _repository = repository;
        }

        public async Task<ColaboradorReadDto> CreateAsync(ColaboradorCreateDto createDto)
        {
            // Mapear DTO -> Entidad
            var colaborador = new Colaborador
            {
                Nombres = createDto.Nombres,
                Apellidos = createDto.Apellidos,
                Area = createDto.Area,
                RolActual = createDto.RolActual,
                Skills = createDto.Skills.Select(s => new ColaboradorSkill
                {
                    SkillId = s.SkillId,
                    NivelId = s.NivelId,
                    Certificaciones = s.Certificaciones
                }).ToList(),
                Disponibilidad = new DisponibilidadColaborador
                {
                    Estado = createDto.Disponibilidad.Estado,
                    Dias = createDto.Disponibilidad.Dias
                }
            };

            var nuevoColaborador = await _repository.CreateAsync(colaborador);

            // Entidad -> ReadDto
            return MapToReadDto(nuevoColaborador);
        }

        public async Task<IEnumerable<ColaboradorReadDto>> GetAllAsync()
        {
            var colaboradores = await _repository.GetAllAsync();

            return colaboradores.Select(MapToReadDto).ToList();
        }

        public async Task<ColaboradorReadDto?> GetByIdAsync(string id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return MapToReadDto(c);
        }

        public async Task<ColaboradorReadDto?> UpdateAsync(string id, ColaboradorUpdateDto updateDto)
        {
            // Verificar si el colaborador existe
            var colaboradorExistente = await _repository.GetByIdAsync(id);
            if (colaboradorExistente == null) return null;

            // Mapear UpdateDto -> Entidad
            var colaboradorActualizado = new Colaborador
            {
                Id = id, // Mantener el mismo ID
                Nombres = updateDto.Nombres,
                Apellidos = updateDto.Apellidos,
                Area = updateDto.Area,
                RolActual = updateDto.RolActual,
                Skills = updateDto.Skills.Select(s => new ColaboradorSkill
                {
                    SkillId = s.SkillId,
                    NivelId = s.NivelId,
                    Certificaciones = s.Certificaciones
                }).ToList(),
                Disponibilidad = new DisponibilidadColaborador
                {
                    Estado = updateDto.Disponibilidad.Estado,
                    Dias = updateDto.Disponibilidad.Dias
                }
            };

            var actualizado = await _repository.UpdateAsync(id, colaboradorActualizado);
            
            if (!actualizado) return null;

            // Entidad -> ReadDto
            return MapToReadDto(colaboradorActualizado);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // Verificar si el colaborador existe
            var colaboradorExistente = await _repository.GetByIdAsync(id);
            if (colaboradorExistente == null) return false;

            return await _repository.DeleteAsync(id);
        }

        // 🔹 Método privado de ayuda para no repetir el mapeo
        private static ColaboradorReadDto MapToReadDto(Colaborador c)
        {
            return new ColaboradorReadDto
            {
                Id = c.Id!,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Area = c.Area,
                RolActual = c.RolActual,
                Skills = c.Skills.Select(s => new ColaboradorSkillReadDto
                {
                    SkillId = s.SkillId,
                    NivelId = s.NivelId,
                    Certificaciones = s.Certificaciones
                }).ToList(),
                Disponibilidad = new DisponibilidadDto
                {
                    Estado = c.Disponibilidad.Estado,
                    Dias = c.Disponibilidad.Dias
                }
            };
        }
    }
}
