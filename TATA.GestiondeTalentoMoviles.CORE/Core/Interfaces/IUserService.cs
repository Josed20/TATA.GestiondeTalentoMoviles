using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(string id);
        Task<UserReadDto?> GetByNombreApellidoAsync(string nombre, string apellido);
        Task<UserReadDto> CreateAsync(UserCreateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<UserReadDto?> UpdateAsync(string id, UserCreateDto dto);
    }
}