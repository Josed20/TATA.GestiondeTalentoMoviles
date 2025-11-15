using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _service;

        public SkillsController(ISkillService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todas las skills
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var skills = await _service.GetAllAsync();
            return Ok(skills);
        }

        /// <summary>
        /// Obtiene una skill por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var skill = await _service.GetByIdAsync(id);
            if (skill == null)
            {
                return NotFound(new { message = $"Skill con ID '{id}' no encontrada" });
            }
            return Ok(skill);
        }

        /// <summary>
        /// Crea una nueva skill
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkillCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nuevaSkill = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = nuevaSkill.Id }, nuevaSkill);
        }

        /// <summary>
        /// Actualiza una skill existente por su ID
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SkillUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var skillActualizada = await _service.UpdateAsync(id, updateDto);
            if (skillActualizada == null)
            {
                return NotFound(new { message = $"Skill con ID '{id}' no encontrada" });
            }

            return Ok(skillActualizada);
        }

        /// <summary>
        /// Elimina una skill por su ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
            {
                return NotFound(new { message = $"Skill con ID '{id}' no encontrada" });
            }

            return NoContent();
        }
    }
}
