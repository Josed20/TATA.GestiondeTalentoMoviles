using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _service;

        public SolicitudesController(ISolicitudService service)
        {
            _service = service;
        }

        /// <summary>
        /// Crea una nueva solicitud (certificación, entrevista de desempeño o actualización de skills)
        /// </summary>
        /// <param name="dto">Datos de la solicitud</param>
        /// <returns>Solicitud creada con su ID asignado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SolicitudReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] SolicitudCreateDto dto)
        {
            try
            {
                // El CreadoPorUsuarioId debe venir en el DTO
                // En producción, esto se obtendría del JWT del usuario autenticado
                if (string.IsNullOrWhiteSpace(dto.CreadoPorUsuarioId))
                {
                    return BadRequest(new { Message = "El campo 'creadoPorUsuarioId' es requerido" });
                }

                var result = await _service.CreateAsync(dto, dto.CreadoPorUsuarioId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las solicitudes
        /// </summary>
        /// <returns>Lista de todas las solicitudes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SolicitudReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _service.GetAllAsync();
            return Ok(solicitudes);
        }

        /// <summary>
        /// Obtiene una solicitud por su ID
        /// </summary>
        /// <param name="id">ID de la solicitud</param>
        /// <returns>Datos de la solicitud</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SolicitudReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var solicitud = await _service.GetByIdAsync(id);
            if (solicitud == null)
                return NotFound(new { Message = $"Solicitud con ID '{id}' no encontrada" });

            return Ok(solicitud);
        }

        /// <summary>
        /// Obtiene todas las solicitudes de un colaborador específico
        /// </summary>
        /// <param name="colaboradorId">ID del colaborador</param>
        /// <returns>Lista de solicitudes del colaborador</returns>
        [HttpGet("colaborador/{colaboradorId}")]
        [ProducesResponseType(typeof(IEnumerable<SolicitudReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByColaborador(string colaboradorId)
        {
            try
            {
                var solicitudes = await _service.GetByColaboradorAsync(colaboradorId);
                return Ok(solicitudes);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el estado de una solicitud (aprobar, rechazar, etc.)
        /// </summary>
        /// <param name="id">ID de la solicitud</param>
        /// <param name="dto">Datos de actualización de estado</param>
        /// <returns>Solicitud actualizada</returns>
        [HttpPut("{id}/estado")]
        [ProducesResponseType(typeof(SolicitudReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEstado(string id, [FromBody] SolicitudUpdateEstadoDto dto)
        {
            try
            {
                // El RevisadoPorUsuarioId debe venir en el DTO
                // En producción, esto se obtendría del JWT del usuario autenticado
                if (string.IsNullOrWhiteSpace(dto.RevisadoPorUsuarioId))
                {
                    return BadRequest(new { Message = "El campo 'revisadoPorUsuarioId' es requerido" });
                }

                var result = await _service.UpdateEstadoAsync(id, dto, dto.RevisadoPorUsuarioId);
                if (result == null)
                    return NotFound(new { Message = $"Solicitud con ID '{id}' no encontrada" });

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una solicitud
        /// </summary>
        /// <param name="id">ID de la solicitud a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { Message = $"Solicitud con ID '{id}' no encontrada" });

                return Ok(new { Message = "Solicitud eliminada correctamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
