using System.ComponentModel.DataAnnotations;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    // Lo que ves de un usuario (ACTUALIZADO)
    public class UserViewDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RolSistema { get; set; }
        public string ColaboradorId { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    // Lo que necesitas para crear un usuario (ACTUALIZADO)
    public class UserCreateDto
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol del sistema es obligatorio")]
        public string RolSistema { get; set; }

        // ID del colaborador asociado (opcional)
        public string ColaboradorId { get; set; }
    }

    // Lo que necesitas para actualizar un usuario
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El rol del sistema es obligatorio")]
        public string RolSistema { get; set; }

        // ID del colaborador asociado (opcional)
        public string ColaboradorId { get; set; }
    }

    // Para cambiar la contraseña del usuario autenticado
    public class UserChangePasswordDto
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; set; }
    }

    // Para que el Admin resetee la contraseña de un usuario
    public class UserResetPasswordDto
    {
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; set; }
    }
}