using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionesController : ControllerBase
    {
        private readonly IEvaluacionService _service;

        // Constructor para inyección de dependencia
        public EvaluacionesController(IEvaluacionService service)
        {
            _service = service;
        }

        // --- GET: /api/Evaluaciones ---
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var evaluaciones = await _service.GetAllAsync();
            return Ok(evaluaciones);
        }

        // --- GET: /api/Evaluaciones/{id} ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var evaluacion = await _service.GetByIdAsync(id);
            if (evaluacion == null)
            {
                return NotFound();
            }
            return Ok(evaluacion);
        }

        // --- POST: /api/Evaluaciones ---
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EvaluacionCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nuevaEvaluacion = await _service.CreateAsync(createDto);

            // Retorna 201 Created y el enlace para acceder al nuevo recurso.
            return CreatedAtAction(nameof(GetById), new { id = nuevaEvaluacion.Id }, nuevaEvaluacion);
        }

        // --- PUT: /api/Evaluaciones/{id} ---
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EvaluacionCreateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Llama al servicio para actualizar el documento
            var result = await _service.UpdateAsync(id, updateDto);

            if (result == null)
            {
                // Si la evaluación no existe, retorna 404
                return NotFound();
            }

            // Retorna 200 OK con el recurso actualizado
            return Ok(result);
        }

        // --- DELETE: /api/Evaluaciones/{id} ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Llama al servicio para eliminar el documento
            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                // Si el documento no existía, retorna 404
                return NotFound();
            }

            // Retorna 204 No Content
            return NoContent();
        }
    }
}