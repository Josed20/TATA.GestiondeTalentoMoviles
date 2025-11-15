using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class RecomendacionService : IRecomendacionService
    {
        private readonly IRecomendacionRepository _repo;

        public RecomendacionService(IRecomendacionRepository repo)
        {
            _repo = repo;
        }

        // ================================================
        //   RECOMENDACIONES DE COLABORADORES
        // ================================================
        public async Task<List<RecomendacionColaboradorDTO>>
            ObtenerRecomendacionesColaboradores(string colaboradorId)
        {
            var data = await _repo.GetRecomendacionesColaboradores(colaboradorId);

            return data.Select(r => new RecomendacionColaboradorDTO
            {
                Id = r.Id,
                ColaboradorId = r.colaboradorId,
                FechaGeneracion = r.FechaGeneracion,
                Recomendaciones = r.Recomendaciones.Select(x => new DetalleRecomendacionDTO
                {
                    RecomendadoId = x.RecomendadoId,
                    Motivo = x.Motivo,
                    CoincidenciaSkills = x.CoincidenciaSkills,
                    ProyectosPrevios = x.ProyectosPrevios,
                    NivelConfianza = x.NivelConfianza

                }).ToList()
            }).ToList();
        }

        // ================================================
        //   RECOMENDACIONES DE VACANTES
        // ================================================
        public async Task<List<RecomendacionVacanteDTO>>
            ObtenerRecomendacionesVacantes(string colaboradorId)
        {
            var data = await _repo.GetRecomendacionesVacantes(colaboradorId);

            return data.Select(r => new RecomendacionVacanteDTO
            {
                Id = r.Id,
                colaboradorId = r.colaboradorId,
                VacanteId = r.VacanteId,
                Motivo = r.Motivo,
                NivelMatch = r.NivelMatch,
                NivelConfianza = r.NivelConfianza

            }).ToList();
        }
    }
}
