using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _repo;

        public AreaService(IAreaRepository repo)
        {
            _repo = repo;
        }

        public async Task<AreaReadDto> CreateAsync(AreaCreateDto dto)
        {
            var area = new Area { Nombre = dto.Nombre };
            var created = await _repo.CreateAsync(area);
            return new AreaReadDto { Id = created.Id!, Nombre = created.Nombre };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<AreaReadDto>> GetAllAsync()
        {
            var areas = await _repo.GetAllAsync();
            return areas.Select(a => new AreaReadDto { Id = a.Id!, Nombre = a.Nombre }).ToList();
        }

        public async Task<AreaReadDto?> GetByIdAsync(string id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return null;
            return new AreaReadDto { Id = a.Id!, Nombre = a.Nombre };
        }

        public async Task<AreaReadDto?> GetByNombreAsync(string nombre)
        {
            var a = await _repo.GetByNombreAsync(nombre);
            if (a == null) return null;
            return new AreaReadDto { Id = a.Id!, Nombre = a.Nombre };
        }

        public async Task<AreaReadDto?> UpdateAsync(string id, AreaCreateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.Nombre = dto.Nombre;
            var updated = await _repo.UpdateAsync(id, existing);
            if (updated == null) return null;
            return new AreaReadDto { Id = updated.Id!, Nombre = updated.Nombre };
        }
    }
}