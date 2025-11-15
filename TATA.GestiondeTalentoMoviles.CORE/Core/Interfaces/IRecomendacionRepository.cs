using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
public interface IRecomendacionRepository
{
    Task<List<RecomendacionColaborador>> GetRecomendacionesColaboradores(string colaboradorId);
    Task<List<RecomendacionVacante>> GetRecomendacionesVacantes(string colaboradorId);
}
