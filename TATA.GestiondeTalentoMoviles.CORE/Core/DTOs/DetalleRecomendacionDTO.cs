using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class DetalleRecomendacionDTO
    {
        public string RecomendadoId { get; set; } = null!;
        public string Motivo { get; set; } = null!;
        public int CoincidenciaSkills { get; set; } = 0;
        public List<string> ProyectosPrevios { get; set; } = null!;
        public string NivelConfianza { get; set; } = null!;
    }
}
