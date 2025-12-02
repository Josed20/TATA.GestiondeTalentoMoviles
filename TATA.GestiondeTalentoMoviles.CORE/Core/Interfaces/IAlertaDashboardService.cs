using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    /// <summary>
    /// Servicio para gestionar el dashboard de alertas unificado
    /// </summary>
    public interface IAlertaDashboardService
    {
        /// <summary>
        /// Obtiene todas las alertas de tipo GAP y ALERTAS pendientes para el administrador
        /// </summary>
        Task<IEnumerable<AlertaDashboardDto>> ObtenerAlertasAdminAsync();

        /// <summary>
        /// Obtiene alertas consolidadas para un colaborador específico
        /// (Certificaciones, Skills GAP, Alertas Genéricas, Vacantes Disponibles)
        /// </summary>
        Task<IEnumerable<AlertaDashboardDto>> ObtenerAlertasColaboradorAsync(string usuarioId);

        /// <summary>
        /// Anuncia una vacante por correo electrónico a colaboradores activos
        /// </summary>
        Task<bool> AnunciarVacantePorCorreoAsync(string vacanteId);
    }
}
