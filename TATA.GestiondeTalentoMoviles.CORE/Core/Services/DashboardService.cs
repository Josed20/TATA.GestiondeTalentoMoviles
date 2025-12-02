using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IMongoCollection<Vacante> _vacantes;
        private readonly IMongoCollection<ProcesosMatching> _procesosMatching;
        private readonly IMongoCollection<Evaluacion> _evaluaciones;

        public DashboardService(IMongoDatabase database)
        {
            _vacantes = database.GetCollection<Vacante>("vacantes");
            _procesosMatching = database.GetCollection<ProcesosMatching>("procesos_matching");
            _evaluaciones = database.GetCollection<Evaluacion>("evaluaciones");
        }

        public async Task<DashboardAdminDto> ObtenerMetricasAdminAsync()
        {
            var dashboard = new DashboardAdminDto
            {
                MetricasVacantes = await ObtenerMetricasVacantesAsync(),
                MetricasMatching = await ObtenerMetricasMatchingAsync(),
                SkillsMasDemandados = await ObtenerSkillsMasDemandadosAsync(),
                BrechasPrioritarias = await ObtenerBrechasPrioritariasAsync(),
                FechaGeneracion = DateTime.UtcNow
            };

            return dashboard;
        }

        private async Task<MetricasVacantesDto> ObtenerMetricasVacantesAsync()
        {
            var vacantes = await _vacantes.Find(_ => true).ToListAsync();
            var totalVacantes = vacantes.Count;
            var vacantesAbiertas = vacantes.Count(v => v.EstadoVacante == "ABIERTA");
            var vacantesCerradas = vacantes.Count(v => v.EstadoVacante == "CERRADA");
            var vacantesEnProceso = vacantes.Count(v => v.EstadoVacante == "EN_PROCESO");
            var vacantesCanceladas = vacantes.Count(v => v.EstadoVacante == "CANCELADA");

            // Calcular tiempo promedio de cobertura para vacantes cerradas
            var vacantesCerradasConFechas = vacantes
                .Where(v => v.EstadoVacante == "CERRADA" && v.FechaCreacion != default && v.FechaActualizacion != default)
                .ToList();

            double tiempoPromedioCobertura = 0;
            if (vacantesCerradasConFechas.Any())
            {
                tiempoPromedioCobertura = vacantesCerradasConFechas
                    .Average(v => (v.FechaActualizacion - v.FechaCreacion).TotalDays);
            }

            return new MetricasVacantesDto
            {
                TotalVacantes = totalVacantes,
                VacantesAbiertas = vacantesAbiertas,
                VacantesCerradas = vacantesCerradas,
                VacantesEnProceso = vacantesEnProceso,
                VacantesCanceladas = vacantesCanceladas,
                TasaCobertura = totalVacantes > 0 ? (double)vacantesCerradas / totalVacantes * 100 : 0,
                TiempoPromedioCoberturaDias = Math.Round(tiempoPromedioCobertura, 2),
                VacantesPorUrgencia = vacantes.GroupBy(v => v.Urgencia)
                    .ToDictionary(g => g.Key, g => g.Count()),
                VacantesPorArea = vacantes.GroupBy(v => v.Area)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        private async Task<MetricasMatchingDto> ObtenerMetricasMatchingAsync()
        {
            var procesos = await _procesosMatching.Find(_ => true).ToListAsync();
            var totalProcesos = procesos.Count;

            if (totalProcesos == 0)
            {
                return new MetricasMatchingDto
                {
                    TotalProcesosEjecutados = 0,
                    PromedioCandidatosPorVacante = 0,
                    PorcentajeMatchPromedio = 0,
                    CandidatosConMatchAlto = 0,
                    CandidatosConMatchMedio = 0,
                    CandidatosConMatchBajo = 0,
                    VacantesSinCandidatos = 0
                };
            }

            var todosCandidatos = procesos.SelectMany(p => p.Candidatos ?? new List<Candidato>()).ToList();
            var promedioCandidatos = (double)todosCandidatos.Count / totalProcesos;

            var candidatosConMatchAlto = todosCandidatos.Count(c => c.PorcentajeMatch > 70);
            var candidatosConMatchMedio = todosCandidatos.Count(c => c.PorcentajeMatch >= 50 && c.PorcentajeMatch <= 70);
            var candidatosConMatchBajo = todosCandidatos.Count(c => c.PorcentajeMatch < 50);

            var porcentajeMatchPromedio = todosCandidatos.Any() 
                ? todosCandidatos.Average(c => c.PorcentajeMatch) 
                : 0;

            var vacantesSinCandidatos = procesos.Count(p => p.Candidatos == null || !p.Candidatos.Any());

            return new MetricasMatchingDto
            {
                TotalProcesosEjecutados = totalProcesos,
                PromedioCandidatosPorVacante = Math.Round(promedioCandidatos, 2),
                PorcentajeMatchPromedio = Math.Round(porcentajeMatchPromedio, 2),
                CandidatosConMatchAlto = candidatosConMatchAlto,
                CandidatosConMatchMedio = candidatosConMatchMedio,
                CandidatosConMatchBajo = candidatosConMatchBajo,
                VacantesSinCandidatos = vacantesSinCandidatos
            };
        }

        private async Task<List<SkillDemandadoDto>> ObtenerSkillsMasDemandadosAsync()
        {
            var vacantes = await _vacantes.Find(_ => true).ToListAsync();
            
            var skillsDemandados = vacantes
                .SelectMany(v => v.SkillsRequeridos ?? new List<SkillRequeridoVacante>())
                .GroupBy(s => new { s.Nombre, s.Tipo })
                .Select(g => new SkillDemandadoDto
                {
                    NombreSkill = g.Key.Nombre,
                    Tipo = g.Key.Tipo,
                    CantidadVacantes = g.Count(),
                    CantidadCriticos = g.Count(s => s.EsCritico),
                    NivelPromedioRequerido = Math.Round(g.Average(s => s.NivelDeseado), 2)
                })
                .OrderByDescending(s => s.CantidadVacantes)
                .Take(10)
                .ToList();

            return skillsDemandados;
        }

        private async Task<List<BrechaSkillDto>> ObtenerBrechasPrioritariasAsync()
        {
            var procesos = await _procesosMatching.Find(_ => true).ToListAsync();
            
            var brechas = procesos
                .SelectMany(p => p.BrechasDetectadas ?? new List<BrechaDetectada>())
                .GroupBy(b => b.Skill)
                .Select(g => new BrechaSkillDto
                {
                    NombreSkill = g.Key,
                    NivelRequeridoPromedio = (int)Math.Round(g.Average(b => b.NivelRequerido)),
                    NivelActualPromedio = Math.Round(g.Average(b => b.PromedioActual), 2),
                    BrechaPromedio = Math.Round(g.Average(b => b.NivelRequerido - b.PromedioActual), 2),
                    ColaboradoresAfectados = (int)Math.Round(g.Average(b => b.CantidadColaboradoresConSkill)),
                    VacantesAfectadas = g.Select(b => b.Skill).Distinct().Count()
                })
                .OrderByDescending(b => b.BrechaPromedio)
                .Take(10)
                .ToList();

            return brechas;
        }
    }
}
