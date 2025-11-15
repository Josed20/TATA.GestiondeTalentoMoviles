using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.DTOs;

public interface IRecomendacionService
{
    Task<List<RecomendacionColaboradorDTO>> ObtenerRecomendacionesColaboradores(string colaboradorId);
    Task<List<RecomendacionVacanteDTO>> ObtenerRecomendacionesVacantes(string colaboradorId);
}
