using System;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs.EvaluacionesII
{
    public class ReadEvaluacionesIIDTO
    {
        public string Id { get; set; } = null!;
        public string TipoEvaluacion { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
