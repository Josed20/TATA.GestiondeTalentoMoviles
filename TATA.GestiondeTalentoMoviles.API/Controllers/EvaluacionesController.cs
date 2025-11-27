using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Entities; // Asegúrate que 'Evaluacion' está aquí
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    // MODIFICACIÓN 1: Ruta explícita para que coincida con la llamada de la app Android ("api/evaluaciones").
    [Route("api/evaluaciones")]
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

        // --- INICIO DE LA MODIFICACIÓN ---
        // MODIFICACIÓN 2: Nuevo endpoint para la carga masiva.
        // La anotación [HttpPost("bulk")] lo asocia a la ruta "POST api/evaluaciones/bulk".
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] List<Evaluacion> evaluaciones)
        {
            if (evaluaciones == null || !evaluaciones.Any())
            {
                return BadRequest("La lista de evaluaciones no puede estar vacía.");
            }

            try
            {
                // Asigna las fechas a todas las evaluaciones de la lista.
                foreach (var evaluacion in evaluaciones)
                {
                    evaluacion.FechaCreacion = DateTime.UtcNow;
                    evaluacion.FechaActualizacion = DateTime.UtcNow;
                }

                // NOTA IMPORTANTE: Para un buen rendimiento, tu `IEvaluacionRepository` debería
                // tener un método que inserte múltiples documentos a la vez (ej. `AddManyAsync`).
                // Si no lo tienes, deberás añadirlo a tu interfaz y repositorio.
                // Por ejemplo: await _repository.AddManyAsync(evaluaciones);

                // Si no tienes un método "AddMany", puedes hacerlo en un bucle,
                // pero será mucho más lento.
                foreach (var evaluacion in evaluaciones)
                {
                    await _repository.AddAsync(evaluacion);
                }


                return Ok(new { message = $"Se han procesado {evaluaciones.Count} evaluaciones correctamente." });
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de que falle la inserción en la base de datos.
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        // --- FIN DE LA MODIFICACIÓN ---

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