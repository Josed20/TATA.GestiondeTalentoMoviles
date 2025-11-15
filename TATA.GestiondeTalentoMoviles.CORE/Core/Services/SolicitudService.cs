using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repo;

        public SolicitudService(ISolicitudRepository repo)
        {
            _repo = repo;
        }

        public async Task<SolicitudActualizacion> CrearSolicitud(SolicitudActualizacion solicitud)
        {
            return await _repo.Create(solicitud);
        }

        public async Task<List<SolicitudActualizacion>> ObtenerPorColaborador(string colaboradorId)
        {
            return await _repo.GetByColaborador(colaboradorId);
        }

        public async Task<SolicitudActualizacion> RevisarSolicitud(string id, string estado, string comentarios)
        {
            return await _repo.UpdateEstado(id, estado, comentarios);
        }
    }


}
