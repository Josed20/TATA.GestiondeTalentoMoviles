using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class UpdateEvaluacionDTO
    {
        public string RolActual { get; set; } = null!;
        public string LiderEvaluador { get; set; } = null!;
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = null!;
        public List<SkillEvaluadoDTO> SkillsEvaluados { get; set; } = new();
        public string? Comentarios { get; set; }
    }
}

