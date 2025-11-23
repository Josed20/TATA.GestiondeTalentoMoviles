using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

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

        // --------------------------------------------
        // GET: Obtener catálogo completo
        // --------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var catalogo = await _service.GetFirstAsync();
            if (catalogo == null)
                return NotFound(new { success = false, message = "Catálogo no encontrado" });

            return Ok(new { success = true, data = catalogo });
        }

        // --------------------------------------------
        // GET: Obtener una sección
        // --------------------------------------------
        [HttpGet("seccion/{nombre}")]
        public async Task<IActionResult> GetSeccion(string nombre)
        {
            var catalogo = await _service.GetFirstAsync();
            if (catalogo == null)
                return NotFound(new { success = false, message = "Catálogo no encontrado" });

            var seccion = GetSection(catalogo, nombre);

            if (seccion == null)
                return BadRequest(new
                {
                    success = false,
                    message = "Sección inválida. Verifique el nombre o cree una sección dinámica."
                });

            return Ok(new { success = true, data = seccion });
        }

        // Función interna para obtener secciones fijas o dinámicas
        private object? GetSection(CatalogoReadDto c, string rawKey)
        {
            string key = rawKey.ToLowerInvariant().Trim();

            var mapping = new Dictionary<string, Func<CatalogoReadDto, object>>()
            {
                { "areas", x => x.Areas! },
                { "roleslaborales", x => x.RolesLaborales! },
                { "roles", x => x.RolesLaborales! },
                { "nivelesskill", x => x.NivelesSkill! },
                { "niveles", x => x.NivelesSkill! },
                { "tiposskill", x => x.TiposSkill! },
                { "tipos", x => x.TiposSkill! }
            };

            if (mapping.ContainsKey(key))
                return mapping[key](c);

            // Secciones dinámicas - parsear el string JSON a JsonElement
            if (c.AdditionalSections != null && c.AdditionalSections.ContainsKey(key))
            {
                var rawJson = c.AdditionalSections[key];
                if (string.IsNullOrWhiteSpace(rawJson))
                    return null;

                try
                {
                    // Parsear el JSON string a JsonElement para que se serialice correctamente
                    using var doc = JsonDocument.Parse(rawJson);
                    return doc.RootElement.Clone();
                }
                catch
                {
                    // Si falla el parseo, devolver null
                    return null;
                }
            }

            return null;
        }

        // --------------------------------------------
        // DELETE: Eliminar un elemento por índice
        // --------------------------------------------
        [Authorize]
        [HttpDelete("{seccion}/{index}")]
        public async Task<IActionResult> DeleteIndex(string seccion, int index)
        {
            if (index < 0)
                return BadRequest(new { success = false, message = "El índice debe ser >= 0" });

            var ok = await _service.DeleteIndexAsync(seccion, index);

            if (!ok)
                return NotFound(new { success = false, message = "Índice o sección no encontrado" });

            return Ok(new { success = true, message = "Elemento eliminado correctamente" });
        }

        // --------------------------------------------
        // PUT: Editar toda una sección completa
        // --------------------------------------------
        [Authorize]
        [HttpPut("{seccion}")]
        public async Task<IActionResult> UpdateSection(string seccion, [FromBody] JsonElement nuevaSeccion)
        {
            try
            {
                var res = await _service.UpdateAsync(seccion, nuevaSeccion);
                return Ok(new { success = true, data = res });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}




