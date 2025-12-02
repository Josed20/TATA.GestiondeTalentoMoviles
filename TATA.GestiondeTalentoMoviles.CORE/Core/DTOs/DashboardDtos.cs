using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    /// <summary>
    /// DTO principal para el dashboard administrativo
    /// </summary>
    public class DashboardAdminDto
    {
        public MetricasVacantesDto MetricasVacantes { get; set; }
        public MetricasMatchingDto MetricasMatching { get; set; }
        public List<SkillDemandadoDto> SkillsMasDemandados { get; set; }
        public List<BrechaSkillDto> BrechasPrioritarias { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }

    /// <summary>
    /// Métricas relacionadas con vacantes
    /// </summary>
    public class MetricasVacantesDto
    {
        public int TotalVacantes { get; set; }
        public int VacantesAbiertas { get; set; }
        public int VacantesCerradas { get; set; }
        public int VacantesEnProceso { get; set; }
        public int VacantesCanceladas { get; set; }
        public double TasaCobertura { get; set; }
        public double TiempoPromedioCoberturaDias { get; set; }
        public Dictionary<string, int> VacantesPorUrgencia { get; set; }
        public Dictionary<string, int> VacantesPorArea { get; set; }
    }

    /// <summary>
    /// Métricas relacionadas con procesos de matching
    /// </summary>
    public class MetricasMatchingDto
    {
        public int TotalProcesosEjecutados { get; set; }
        public double PromedioCandidatosPorVacante { get; set; }
        public double PorcentajeMatchPromedio { get; set; }
        public int CandidatosConMatchAlto { get; set; }
        public int CandidatosConMatchMedio { get; set; }
        public int CandidatosConMatchBajo { get; set; }
        public int VacantesSinCandidatos { get; set; }
    }

    /// <summary>
    /// Skill más demandado en vacantes
    /// </summary>
    public class SkillDemandadoDto
    {
        public string NombreSkill { get; set; }
        public string Tipo { get; set; }
        public int CantidadVacantes { get; set; }
        public int CantidadCriticos { get; set; }
        public double NivelPromedioRequerido { get; set; }
    }

    /// <summary>
    /// Brecha de skill detectada
    /// </summary>
    public class BrechaSkillDto
    {
        public string NombreSkill { get; set; }
        public int NivelRequeridoPromedio { get; set; }
        public double NivelActualPromedio { get; set; }
        public double BrechaPromedio { get; set; }
        public int ColaboradoresAfectados { get; set; }
        public int VacantesAfectadas { get; set; }
    }
}
