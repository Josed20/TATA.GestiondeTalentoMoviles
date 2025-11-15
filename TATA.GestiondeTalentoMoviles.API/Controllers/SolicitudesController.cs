using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _service;

        public SolicitudesController(ISolicitudService service)
        {
            _service = service;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] SolicitudActualizacionDTO dto)
        {
            var solicitud = new SolicitudActualizacion
            {
                ColaboradorId = dto.ColaboradorId,
                SkillId = dto.SkillId,
                NuevoNivel = dto.NuevoNivel,
                EvidenciaUrl = dto.EvidenciaUrl,
                ComentariosRRHH = dto.ComentariosRRHH,
                Estado = "Pendiente",
                FechaSolicitud = DateTime.UtcNow,
                FechaRevision = null
            };

            var result = await _service.CrearSolicitud(solicitud);
            return Ok(result);
        }


        [HttpGet("colaborador/{id}")]
        public async Task<IActionResult> GetByColaborador(string id)
        {
            var r = await _service.ObtenerPorColaborador(id);
            return Ok(r);
        }

        [HttpPut("revisar/{id}")]
        public async Task<IActionResult> Revisar(
            string id,
            [FromQuery] string estado,
            [FromQuery] string comentarios)
        {
            var r = await _service.RevisarSolicitud(id, estado, comentarios);
            return Ok(r);
        }
    }
}
