using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    // ============================================================
    // VIEW DTO
    // ============================================================
    public class AlertaViewDto
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public string ColaboradorId { get; set; }
        public string VacanteId { get; set; }
        public string ProcesoMatchingId { get; set; }
        public AlertaDetalleDto Detalle { get; set; }
        public List<AlertaDestinatarioDto> Destinatarios { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string UsuarioResponsable { get; set; }
    }

    public class AlertaDetalleDto
    {
        public DateTime? FechaProximaEvaluacion { get; set; }
        public List<SkillFaltanteDto> SkillsFaltantes { get; set; }
        public string Descripcion { get; set; }
    }

    public class SkillFaltanteDto
    {
        public string Nombre { get; set; }
        public int NivelRequerido { get; set; }
    }

    public class AlertaDestinatarioDto
    {
        public string UsuarioId { get; set; }
        public string Tipo { get; set; }
    }

    // ============================================================
    // CREATE DTO
    // ============================================================
    public class AlertaCreateDto
    {
        [Required]
        public string Tipo { get; set; }

        [Required]
        public string Estado { get; set; }

        public string ColaboradorId { get; set; }
        public string VacanteId { get; set; }
        public string ProcesoMatchingId { get; set; }

        [Required]
        public AlertaDetalleCreateDto Detalle { get; set; }

        [Required]
        public List<AlertaDestinatarioCreateDto> Destinatarios { get; set; }

        [Required]
        public string UsuarioResponsable { get; set; }
    }

    public class AlertaDetalleCreateDto
    {
        public DateTime? FechaProximaEvaluacion { get; set; }
        public List<SkillFaltanteDto> SkillsFaltantes { get; set; }
        public string Descripcion { get; set; }
    }

    public class AlertaDestinatarioCreateDto
    {
        public string UsuarioId { get; set; }
        public string Tipo { get; set; }
    }

    // ============================================================
    // UPDATE DTO
    // ============================================================
    public class AlertaUpdateDto
    {
        [Required]
        public string Tipo { get; set; }

        [Required]
        public string Estado { get; set; }

        public string VacanteId { get; set; }
        public string ProcesoMatchingId { get; set; }

        [Required]
        public AlertaDetalleUpdateDto Detalle { get; set; }

        [Required]
        public List<AlertaDestinatarioUpdateDto> Destinatarios { get; set; }

        [Required]
        public string UsuarioResponsable { get; set; }
    }

    public class AlertaDetalleUpdateDto
    {
        public DateTime? FechaProximaEvaluacion { get; set; }
        public List<SkillFaltanteDto> SkillsFaltantes { get; set; }
        public string Descripcion { get; set; }
    }

    public class AlertaDestinatarioUpdateDto
    {
        public string UsuarioId { get; set; }
        public string Tipo { get; set; }
    }
}
