using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(AuthRequestDto request);
        Task ChangePasswordAsync(string userId, UserChangePasswordDto dto);
    }
}