using System.ComponentModel.DataAnnotations;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    // Petición de Login
    public class AuthRequestDto
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }

    // Respuesta del Login (ACTUALIZADA)
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string RolSistema { get; set; } // Ajustado a string
        public string ColaboradorId { get; set; }
    }
}