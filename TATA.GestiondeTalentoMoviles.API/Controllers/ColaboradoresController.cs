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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colaboradores = await _service.GetAllAsync();
            return Ok(colaboradores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var colaborador = await _service.GetByIdAsync(id);
            if (colaborador == null)
            {
                return NotFound();
            }
            return Ok(colaborador);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ColaboradorCreateDto createDto)
        {
            var nuevoColaborador = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = nuevoColaborador.Id }, nuevoColaborador);
        }
    }
}
