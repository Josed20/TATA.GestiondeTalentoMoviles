using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class RecomendacionVacanteDTO
    {
        public string? Id { get; set; } = null!;

        public string colaboradorId { get; set; } = null!;
        public string VacanteId { get; set; } = null!;
        public string Motivo { get; set; } = null!;
        public int NivelMatch { get; set; } = 0;
        public string NivelConfianza { get; set; } = null!;
    }
}
