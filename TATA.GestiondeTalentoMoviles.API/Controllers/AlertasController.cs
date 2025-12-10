using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.Constants;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/alertas")]
    [ApiController]
    [Authorize(Roles = AppRoles.ADMIN + "," + AppRoles.COLABORADOR)]
    public class AlertasController : ControllerBase
    {
        private readonly IAlertaService _service;
        private readonly IAlertaDashboardService _dashboardService;

        public AlertasController(IAlertaService service, IAlertaDashboardService dashboardService)
        {
            _service = service;
            _dashboardService = dashboardService;
        }

        // ============================================================
        // ENDPOINTS CRUD TRADICIONALES
        // ============================================================

        /// <summary>
        /// Lista todas las alertas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(new { success = true, data });
        }

        /// <summary>
        /// Obtiene una alerta por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var alerta = await _service.GetByIdAsync(id);

            if (alerta == null)
                return NotFound(new { success = false, message = "Alerta no encontrada" });

            return Ok(new { success = true, data = alerta });
        }

        /// <summary>
        /// Obtiene alertas de un colaborador específico
        /// </summary>
        [HttpGet("colaborador/{colaboradorId}")]
        public async Task<IActionResult> GetByColaborador(string colaboradorId)
        {
            var alertas = await _service.GetAlertasPorColaboradorAsync(colaboradorId);
            return Ok(new { success = true, data = alertas });
        }

        /// <summary>
        /// Obtiene todas las alertas pendientes
        /// </summary>
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            var alertas = await _service.GetAlertasPendientesAsync();
            return Ok(new { success = true, data = alertas });
        }

        /// <summary>
        /// Crea una nueva alerta
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AlertaCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Error de validación",
                        errors = ModelState
                    });
                }

                var nuevaAlerta = await _service.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById),
                    new { id = nuevaAlerta.Id },
                    new { success = true, data = nuevaAlerta });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una alerta existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AlertaUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Error de validación",
                        errors = ModelState
                    });
                }

                var updated = await _service.UpdateAsync(id, dto);

                return Ok(new
                {
                    success = true,
                    message = "Alerta actualizada exitosamente",
                    data = updated
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una alerta
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Alerta eliminada exitosamente"
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ENDPOINTS DE DASHBOARD UNIFICADO
        // ============================================================

        /// <summary>
        /// [DASHBOARD] Obtiene todas las alertas de tipo GAP y ALERTAS pendientes para el administrador.
        /// Consolida información de Evaluaciones (Skills GAP) y Alertas Genéricas pendientes.
        /// </summary>
        /// <returns>Lista de alertas para monitoreo administrativo ordenadas por prioridad</returns>
        [HttpGet("dashboard/admin")]
        [Authorize(Roles = AppRoles.ADMIN)]
        public async Task<IActionResult> GetDashboardAdmin()
        {
            try
            {
                var alertas = await _dashboardService.ObtenerAlertasAdminAsync();
                
                return Ok(new
                {
                    success = true,
                    message = "Alertas administrativas obtenidas exitosamente",
                    data = alertas
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error al obtener alertas administrativas: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// [DASHBOARD] Obtiene alertas consolidadas para un colaborador específico (Vista Unificada).
        /// Incluye: Certificaciones por vencer/vencidas, Skills GAP, Alertas Genéricas, Vacantes Disponibles.
        /// </summary>
        /// <param name="usuarioId">ID del usuario/colaborador</param>
        /// <returns>Lista ordenada por prioridad (ROJO > AMARILLO > VERDE) con metadata</returns>
        [HttpGet("dashboard/colaborador/{usuarioId}")]
        [Authorize(Roles = $"{AppRoles.ADMIN},{AppRoles.COLABORADOR}")]
        public async Task<IActionResult> GetDashboardColaborador(string usuarioId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El ID del usuario es requerido"
                    });
                }

                var alertas = await _dashboardService.ObtenerAlertasColaboradorAsync(usuarioId);
                
                return Ok(new
                {
                    success = true,
                    message = "Alertas del colaborador obtenidas exitosamente",
                    data = alertas,
                    metadata = new
                    {
                        usuarioId,
                        totalAlertas = System.Linq.Enumerable.Count(alertas),
                        rojas = System.Linq.Enumerable.Count(System.Linq.Enumerable.Where(alertas, a => a.ColorPrioridad == "ROJO")),
                        amarillas = System.Linq.Enumerable.Count(System.Linq.Enumerable.Where(alertas, a => a.ColorPrioridad == "AMARILLO")),
                        verdes = System.Linq.Enumerable.Count(System.Linq.Enumerable.Where(alertas, a => a.ColorPrioridad == "VERDE"))
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error al obtener alertas del colaborador: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// [DASHBOARD] Envía correo electrónico a colaboradores activos anunciando una vacante.
        /// Valida que la vacante esté ABIERTA y envía correo HTML profesional con toda la información.
        /// </summary>
        /// <param name="dto">Objeto con el ID de la vacante a anunciar</param>
        /// <returns>Confirmación de envío de correos exitoso</returns>
        [HttpPost("dashboard/anunciar-vacante")]
        [Authorize(Roles = AppRoles.ADMIN)]
        public async Task<IActionResult> AnunciarVacante([FromBody] AnunciarVacanteDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto?.VacanteId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "El vacanteId es requerido"
                    });
                }

                var resultado = await _dashboardService.AnunciarVacantePorCorreoAsync(dto.VacanteId);

                if (resultado)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Vacante anunciada exitosamente por correo electrónico",
                        data = new
                        {
                            vacanteId = dto.VacanteId,
                            fechaEnvio = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo enviar el anuncio de la vacante"
                    });
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al anunciar vacante: {ex.Message}"
                });
            }
        }
    }
}
