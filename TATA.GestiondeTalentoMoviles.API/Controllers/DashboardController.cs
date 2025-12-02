using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Constants;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.ADMIN)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Obtiene todas las métricas consolidadas para el dashboard administrativo
        /// </summary>
        /// <returns>Métricas de vacantes, matching, skills demandados y brechas</returns>
        [HttpGet("metricas-admin")]
        public async Task<IActionResult> ObtenerMetricasAdmin()
        {
            try
            {
                var dashboard = await _dashboardService.ObtenerMetricasAdminAsync();

                return Ok(new
                {
                    success = true,
                    message = "Métricas del dashboard obtenidas exitosamente",
                    data = dashboard
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al obtener métricas del dashboard: {ex.Message}"
                });
            }
        }
    }
}
