using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.Constants;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/alertas")]
    [ApiController]
    [Authorize(Roles = AppRoles.ADMIN)]
    public class AlertasController : ControllerBase
    {
        private readonly IAlertaService _service;

        public AlertasController(IAlertaService service)
        {
            _service = service;
        }

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
    }
}
