using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces
{
    public interface ISolicitudRepository
    {
        Task<List<SolicitudActualizacion>> GetByColaborador(string colaboradorId);
        Task<SolicitudActualizacion> Create(SolicitudActualizacion solicitud);
        Task<SolicitudActualizacion> UpdateEstado(string id, string estado, string comentarios);
    }
}
