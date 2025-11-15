using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class SolicitudRevisionDTO
    {
        public string Estado { get; set; } // Aprobado | Rechazado
        public string ComentariosRRHH { get; set; }
    }
}
