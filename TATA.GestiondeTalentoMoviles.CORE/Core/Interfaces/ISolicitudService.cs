using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface ISolicitudService
    {
        Task<SolicitudActualizacion> CrearSolicitud(SolicitudActualizacion solicitud);
        Task<List<SolicitudActualizacion>> ObtenerPorColaborador(string colaboradorId);
        Task<SolicitudActualizacion> RevisarSolicitud(string id, string estado, string comentarios);
    }
}
