using System.ComponentModel.DataAnnotations;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public record RoleResponseDto(
        string Id,
        string Nombre
    );

    public record CreateRoleDto(
        [Required(ErrorMessage = "El nombre del rol es requerido")]
        string Nombre
    );

    public record UpdateRoleDto(
        string Nombre
    );
}
