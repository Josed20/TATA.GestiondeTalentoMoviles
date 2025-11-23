using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluacionesIIController : ControllerBase
    {
        private readonly IEvaluacionesIIRepository _repository;

        public EvaluacionesIIController(IEvaluacionesIIRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EvaluacionesII plantilla)
        {
            plantilla.FechaCreacion = DateTime.UtcNow;
            plantilla.FechaActualizacion = DateTime.UtcNow;

            await _repository.AddAsync(plantilla);
            return CreatedAtAction(nameof(GetById), new { id = plantilla.Id }, plantilla);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, EvaluacionesII plantilla)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            plantilla.Id = id;
            plantilla.FechaActualizacion = DateTime.UtcNow;

            await _repository.UpdateAsync(plantilla);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
