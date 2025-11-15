using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class RecomendacionesController : ControllerBase
{
    private readonly IRecomendacionService _service;

    public RecomendacionesController(IRecomendacionService service)
    {
        _service = service;
    }

    [HttpGet("colaboradores/{id}")]
    public async Task<IActionResult> GetColaboradores(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out var objectId))
            return BadRequest("El ID no es un ObjectId válido.");

        var result = await _service.ObtenerRecomendacionesColaboradores(objectId.ToString());

        return Ok(result);
    }



    [HttpGet("vacantes/{colaboradorId}")]
    public async Task<IActionResult> ObtenerRecomendacionesVacantes(string colaboradorId)
    {
        var data = await _service.ObtenerRecomendacionesVacantes(colaboradorId);
        return Ok(data);
    }


}
