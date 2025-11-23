using System;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class SkillEvaluadoDTO
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!; // TECNICO / BLANDO
        public int NivelActual { get; set; }
        public int NivelRecomendado { get; set; }
    }
}
