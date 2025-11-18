using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserViewDto> CreateUserAsync(UserCreateDto userDto);
        Task<IEnumerable<UserViewDto>> GetAllUsersAsync();
        Task<UserViewDto> GetUserByIdAsync(string id);
        Task<UserViewDto> UpdateUserAsync(string id, UserUpdateDto dto);
        Task DeleteUserAsync(string id);
        Task ResetPasswordAsync(string id, UserResetPasswordDto dto);
        Task UnblockUserAsync(string id);
    }
}