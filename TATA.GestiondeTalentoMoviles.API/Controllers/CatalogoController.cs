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

            var key = nombre?.ToLowerInvariant();
            return key switch
            {
                "areas" => Ok(new { success = true, data = catalogo.Areas }),
                "roleslaborales" or "roles" => Ok(new { success = true, data = catalogo.RolesLaborales }),
                "nivelesskill" or "niveles" or "nivelesskills" => Ok(new { success = true, data = catalogo.NivelesSkill }),
                "tiposskill" or "tiposs" or "tipos" => Ok(new { success = true, data = catalogo.TiposSkill }),
                _ => catalogo.AdditionalSections != null && catalogo.AdditionalSections.ContainsKey(key!) ? Ok(new { success = true, data = catalogo.AdditionalSections[key!] }) : BadRequest(new { success = false, message = "Sección inválida. Use: areas, rolesLaborales, nivelesSkill, tiposSkill o una sección dinámica existente" })
            };
        }

        [Authorize]
        [HttpPut("{seccion}")]
        public async Task<IActionResult> UpdateBySeccion(string seccion, [FromBody] object data)
        {
            if (data == null)
                return BadRequest(new { success = false, message = "Payload inválido" });

            try
            {
                var updated = await _service.UpdateAsync(seccion, data);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{seccion}/{index}")]
        public async Task<IActionResult> DeleteIndex(string seccion, int index)
        {
            var ok = await _service.DeleteIndexAsync(seccion, index);
            if (!ok)
                return NotFound(new { success = false, message = "Índice o sección no encontrado" });

            return Ok(new { success = true, message = "Elemento eliminado" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSection([FromBody] CatalogoCreateSectionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Payload inválido" });

            try
            {
                var res = await _service.CreateSectionAsync(dto);
                return Created("/api/catalogo", new { success = true, data = res });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{seccion}")]
        public async Task<IActionResult> AddItemToSection(string seccion, [FromBody] object item)
        {
            if (item == null)
                return BadRequest(new { success = false, message = "Payload inválido" });

            var res = await _service.AddItemToSectionAsync(seccion, item);
            return Ok(new { success = true, data = res });
        }
    }
}
