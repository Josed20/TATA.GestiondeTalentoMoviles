using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new RoleResponseDto(r.Id!, r.Nombre));
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(string id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return null;

            return new RoleResponseDto(role.Id!, role.Nombre);
        }

        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = new Role
            {
                Nombre = dto.Nombre
            };

            await _roleRepository.CreateAsync(role);
            return new RoleResponseDto(role.Id!, role.Nombre);
        }

        public async Task<bool> UpdateRoleAsync(string id, UpdateRoleDto dto)
        {
            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
                return false;

            existingRole.Nombre = dto.Nombre;
            return await _roleRepository.UpdateAsync(id, existingRole);
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            return await _roleRepository.DeleteAsync(id);
        }
    }
}
