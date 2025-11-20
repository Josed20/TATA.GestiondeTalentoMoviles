using System.Linq;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly ICatalogoRepository _repo;

        public CatalogoService(ICatalogoRepository repo)
        {
            _repo = repo;
        }

        public async Task<CatalogoReadDto?> GetAsync(string id)
        {
            var c = await _repo.GetAsync(id);
            if (c == null) return null;
            return Map(c);
        }

        public async Task<CatalogoReadDto?> GetFirstAsync()
        {
            var c = await _repo.GetFirstAsync();
            if (c == null) return null;
            return Map(c);
        }

        public async Task<CatalogoReadDto> UpdateAsync(string id, CatalogoUpdateDto dto)
        {
            var existing = await _repo.GetAsync(id);
            if (existing == null)
            {
                // Crear nuevo con id
                var nuevo = new Catalogo
                {
                    Id = id,
                    Areas = dto.Areas ?? new(),
                    RolesLaborales = dto.RolesLaborales ?? new(),
                    NivelesSkill = dto.NivelesSkill?.Select(n => new NivelSkill { Codigo = n.Codigo, Descripcion = n.Descripcion }).ToList() ?? new(),
                    TiposSkill = dto.TiposSkill ?? new()
                };
                await _repo.CreateOrReplaceAsync(nuevo);
                return Map(nuevo);
            }

            // Actualizar solo campos no nulos del DTO
            existing.Areas = dto.Areas ?? existing.Areas;
            existing.RolesLaborales = dto.RolesLaborales ?? existing.RolesLaborales;
            if (dto.NivelesSkill != null)
                existing.NivelesSkill = dto.NivelesSkill.Select(n => new NivelSkill { Codigo = n.Codigo, Descripcion = n.Descripcion }).ToList();
            existing.TiposSkill = dto.TiposSkill ?? existing.TiposSkill;

            await _repo.CreateOrReplaceAsync(existing);
            return Map(existing);
        }

        private static CatalogoReadDto Map(Catalogo c)
        {
            return new CatalogoReadDto
            {
                Id = c.Id,
                Areas = c.Areas,
                RolesLaborales = c.RolesLaborales,
                NivelesSkill = c.NivelesSkill.Select(n => new NivelSkillDto { Codigo = n.Codigo, Descripcion = n.Descripcion }).ToList(),
                TiposSkill = c.TiposSkill
            };
        }
    }
}