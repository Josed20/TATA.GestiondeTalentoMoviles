using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NivelesSkillController : ControllerBase
    {
        private readonly INivelSkillService _service;

        public NivelesSkillController(INivelSkillService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todos los niveles de skill
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var niveles = await _service.GetAllAsync();
            return Ok(niveles);
        }

        /// <summary>
        /// Obtiene un nivel de skill por su ID (ObjectId)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var nivel = await _service.GetByIdAsync(id);
            if (nivel == null)
            {
                return NotFound(new { message = $"Nivel de skill con ID '{id}' no encontrado" });
            }
            return Ok(nivel);
        }

        /// <summary>
        /// Crea un nuevo nivel de skill
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NivelSkillCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoNivel = await _service.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = nuevoNivel.Id }, nuevoNivel);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe"))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un nivel de skill existente por su ID (ObjectId)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NivelSkillUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nivelActualizado = await _service.UpdateAsync(id, updateDto);
            if (nivelActualizado == null)
            {
                return NotFound(new { message = $"Nivel de skill con ID '{id}' no encontrado" });
            }

            return Ok(nivelActualizado);
        }

        /// <summary>
        /// Elimina un nivel de skill por su ID (ObjectId)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
            {
                return NotFound(new { message = $"Nivel de skill con ID '{id}' no encontrado" });
            }

            return NoContent();
        }
    }
}
