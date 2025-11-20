using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColaboradoresController : ControllerBase
    {
        private readonly IColaboradorService _service;

        public ColaboradoresController(IColaboradorService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todos los colaboradores
        /// </summary>
        /// <returns>Lista de todos los colaboradores</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ColaboradorReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var colaboradores = await _service.GetAllAsync();
            return Ok(colaboradores);
        }

        /// <summary>
        /// Obtiene un colaborador por su ID
        /// </summary>
        /// <param name="id">ID del colaborador</param>
        /// <returns>Datos del colaborador</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ColaboradorReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var colaborador = await _service.GetByIdAsync(id);
            if (colaborador == null)
            {
                return NotFound(new { message = $"Colaborador con ID '{id}' no encontrado" });
            }
            return Ok(colaborador);
        }

        /// <summary>
        /// Crea un nuevo colaborador
        /// </summary>
        /// <param name="createDto">Datos del nuevo colaborador</param>
        /// <returns>Colaborador creado con su ID asignado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ColaboradorReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ColaboradorCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nuevoColaborador = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = nuevoColaborador.Id }, nuevoColaborador);
        }

        /// <summary>
        /// Actualiza un colaborador existente por su ID
        /// </summary>
        /// <param name="id">ID del colaborador a actualizar</param>
        /// <param name="updateDto">Datos actualizados del colaborador</param>
        /// <returns>Colaborador actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ColaboradorReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] ColaboradorUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var colaboradorActualizado = await _service.UpdateAsync(id, updateDto);
            if (colaboradorActualizado == null)
            {
                return NotFound(new { message = $"Colaborador con ID '{id}' no encontrado" });
            }

            return Ok(colaboradorActualizado);
        }

        /// <summary>
        /// Elimina lógicamente un colaborador (marca como INACTIVO)
        /// </summary>
        /// <param name="id">ID del colaborador a eliminar</param>
        /// <returns>No content si se elimina correctamente</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
            {
                return NotFound(new { message = $"Colaborador con ID '{id}' no encontrado" });
            }

            return NoContent();
        }
    }
}
