using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync();
        Task<RoleResponseDto?> GetRoleByIdAsync(string id);
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto);
        Task<bool> UpdateRoleAsync(string id, UpdateRoleDto dto);
        Task<bool> DeleteRoleAsync(string id);
    }
}
