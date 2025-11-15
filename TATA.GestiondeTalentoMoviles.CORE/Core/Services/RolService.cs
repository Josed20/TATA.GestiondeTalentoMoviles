using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _repo;

        public RolService(IRolRepository repo)
        {
            _repo = repo;
        }

        public async Task<RolReadDto> CreateAsync(RolCreateDto dto)
        {
            var rol = new Rol { Nombre = dto.Nombre };
            var created = await _repo.CreateAsync(rol);
            return new RolReadDto { Id = created.Id!, Nombre = created.Nombre };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<RolReadDto>> GetAllAsync()
        {
            var roles = await _repo.GetAllAsync();
            return roles.Select(r => new RolReadDto { Id = r.Id!, Nombre = r.Nombre }).ToList();
        }

        public async Task<RolReadDto?> GetByIdAsync(string id)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return null;
            return new RolReadDto { Id = r.Id!, Nombre = r.Nombre };
        }

        public async Task<RolReadDto?> GetByNombreAsync(string nombre)
        {
            var r = await _repo.GetByNombreAsync(nombre);
            if (r == null) return null;
            return new RolReadDto { Id = r.Id!, Nombre = r.Nombre };
        }

        public async Task<RolReadDto?> UpdateAsync(string id, RolCreateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.Nombre = dto.Nombre;
            var updated = await _repo.UpdateAsync(id, existing);
            if (updated == null) return null;
            return new RolReadDto { Id = updated.Id!, Nombre = updated.Nombre };
        }
    }
}