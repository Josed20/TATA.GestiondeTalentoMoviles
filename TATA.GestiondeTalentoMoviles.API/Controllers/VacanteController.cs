using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacantesController : ControllerBase
    {
        private readonly IVacanteService _service;

        public VacantesController(IVacanteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vacantes = await _service.GetAllAsync();
            return Ok(vacantes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var vacante = await _service.GetByIdAsync(id);
            if (vacante == null) return NotFound();
            return Ok(vacante);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VacanteCreateDto createDto)
        {
            var nuevaVacante = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = nuevaVacante.Id }, nuevaVacante);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] VacanteCreateDto updateDto)
        {
            var updated = await _service.UpdateAsync(id, updateDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
