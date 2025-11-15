using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

        /// <summary>
        /// Autentica a un usuario y devuelve los tokens
        /// </summary>
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);

        /// <summary>
        /// Refresca el token de acceso usando el refresh token
        /// </summary>
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
