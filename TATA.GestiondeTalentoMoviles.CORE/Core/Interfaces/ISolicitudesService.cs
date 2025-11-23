using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ISolicitudService
    {
        // POST - Crear solicitud
        Task<SolicitudReadDto> CreateAsync(SolicitudCreateDto dto, string creadoPorUsuarioId);

        // GET - Listar todas
        Task<IEnumerable<SolicitudReadDto>> GetAllAsync();

        // GET - Detalle por Id
        Task<SolicitudReadDto?> GetByIdAsync(string id);

        // GET - Todas las solicitudes de un colaborador
        Task<IEnumerable<SolicitudReadDto>> GetByColaboradorAsync(string colaboradorId);

        // PUT - Actualizar estado (APROBADA / RECHAZADA / ...)
        Task<SolicitudReadDto?> UpdateEstadoAsync(string id, SolicitudUpdateEstadoDto dto, string revisadoPorUsuarioId);

        // DELETE - Eliminar solicitud
        Task<bool> DeleteAsync(string id);
    }
}
