using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colaboradores = await _service.GetAllAsync();
            return Ok(colaboradores);
        }

        /// <summary>
        /// Obtiene un colaborador por su ID
        /// </summary>
        [HttpGet("{id}")]
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
        [HttpPost]
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
        [HttpPut("{id}")]
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
        /// Elimina (marca como inactivo) un colaborador por su ID
        /// </summary>
        [HttpDelete("{id}")]
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
