using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IDashboardService
    {
        /// <summary>
        /// Obtiene todas las métricas consolidadas para el dashboard administrativo
        /// </summary>
        Task<DashboardAdminDto> ObtenerMetricasAdminAsync();
    }
}
