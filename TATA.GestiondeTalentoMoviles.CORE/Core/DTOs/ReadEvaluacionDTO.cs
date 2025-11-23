using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class ReadEvaluacionDTO
    {
        public string Id { get; set; } = null!;
        public string ColaboradorId { get; set; } = null!;
        public string RolActual { get; set; } = null!;
        public string LiderEvaluador { get; set; } = null!;
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = null!;
        public List<SkillEvaluadoDTO> SkillsEvaluados { get; set; } = new();
        public string? Comentarios { get; set; }
        public string UsuarioResponsable { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}

