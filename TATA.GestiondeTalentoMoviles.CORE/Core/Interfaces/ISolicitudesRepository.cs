using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ISolicitudesRepository
    {
        Task<Solicitud> CreateAsync(Solicitud solicitud);
        Task<IEnumerable<Solicitud>> GetAllAsync();
        Task<Solicitud?> GetByIdAsync(string id);
        Task<IEnumerable<Solicitud>> GetByColaboradorAsync(string colaboradorId);
        Task<Solicitud?> UpdateEstadoAsync(string id, string estadoSolicitud, string? observacionAdmin, string? revisadoPorUsuarioId);
        Task<bool> DeleteAsync(string id);
    }
}
