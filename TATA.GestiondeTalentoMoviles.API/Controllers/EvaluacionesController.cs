using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.INFRASTRUCTURE.Repositories;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluacionesController : ControllerBase
    {
        private readonly IEvaluacionRepository _repository;

        public EvaluacionesController(IEvaluacionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var evaluaciones = await _repository.GetAllAsync();
            return Ok(evaluaciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var evaluacion = await _repository.GetByIdAsync(id);
            if (evaluacion == null) return NotFound();
            return Ok(evaluacion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Evaluacion evaluacion)
        {
            evaluacion.FechaCreacion = DateTime.UtcNow;
            evaluacion.FechaActualizacion = DateTime.UtcNow;

            await _repository.AddAsync(evaluacion);
            return CreatedAtAction(nameof(GetById), new { id = evaluacion.Id }, evaluacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Evaluacion evaluacion)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            evaluacion.Id = id;
            evaluacion.FechaActualizacion = DateTime.UtcNow;

            await _repository.UpdateAsync(evaluacion);
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
