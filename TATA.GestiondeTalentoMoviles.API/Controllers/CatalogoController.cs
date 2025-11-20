using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [ApiController]
    [Route("api/catalogo")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _service;

        public CatalogoController(ICatalogoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Devolver el primer documento (se asume un único catalogo global)
            var catalogo = await _service.GetFirstAsync();
            if (catalogo == null)
                return NotFound(new { success = false, message = "Catálogo no encontrado" });

            return Ok(new { success = true, data = catalogo });
        }

        [HttpGet("seccion/{nombre}")]
        public async Task<IActionResult> GetSeccion(string nombre)
        {
            var catalogo = await _service.GetFirstAsync();
            if (catalogo == null)
                return NotFound(new { success = false, message = "Catálogo no encontrado" });

            // Normalizar
            var key = nombre?.ToLowerInvariant();
            return key switch
            {
                "areas" => Ok(new { success = true, data = catalogo.Areas }),
                "roleslaborales" or "roles" => Ok(new { success = true, data = catalogo.RolesLaborales }),
                "nivelesskill" or "nivelesskill" or "niveles" => Ok(new { success = true, data = catalogo.NivelesSkill }),
                "tiposskill" or "tiposs" or "tipos" => Ok(new { success = true, data = catalogo.TiposSkill }),
                _ => BadRequest(new { success = false, message = "Sección inválida. Use: areas, rolesLaborales, nivelesSkill, tiposSkill" })
            };
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CatalogoUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "Payload inválido" });

            var updated = await _service.UpdateAsync(id, dto);
            return Ok(new { success = true, data = updated });
        }
    }
}
