using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class SolicitudActualizacionDTO
    {
        public string ColaboradorId { get; set; } = null!;
        public string SkillId { get; set; } = null!;
        public string NuevoNivel { get; set; } = null!;
        public string EvidenciaUrl { get; set; } = null!;
        public string? ComentariosRRHH { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
        public DateTime? FechaRevision { get; set; }
    }
}
