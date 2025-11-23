using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class ProcesosMatchingViewDto
    {
        public string Id { get; set; }
        public string VacanteId { get; set; }
        public string EjecutadoPorUsuarioId { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public double UmbralMatch { get; set; }
        public List<CandidatoDto> Candidatos { get; set; }
        public List<BrechaDetectadaDto> BrechasDetectadas { get; set; }
        public string ResultadoGlobal { get; set; }
        public string MensajeSistema { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ProcesosMatchingCreateDto
    {
        [Required]
        public string VacanteId { get; set; }

        [Required]
        public string EjecutadoPorUsuarioId { get; set; }

        public DateTime? FechaEjecucion { get; set; }

        [Range(0, 1)]
        public double UmbralMatch { get; set; }

        public List<CandidatoDto> Candidatos { get; set; } = new();

        public List<BrechaDetectadaDto> BrechasDetectadas { get; set; } = new();

        public string ResultadoGlobal { get; set; }

        public string MensajeSistema { get; set; }
    }

    public class ProcesosMatchingUpdateDto : ProcesosMatchingCreateDto
    {
        // Reutiliza campos de creación; fechaCreacion no se actualiza desde cliente.
    }

    public class CandidatoDto
    {
        [Required]
        public string ColaboradorId { get; set; }

        [Required]
        public string NombreColaborador { get; set; }

        public double PorcentajeMatch { get; set; }

        public List<DetalleMatchDto> DetalleMatch { get; set; } = new();

        public string Disponibilidad { get; set; }
    }

    public class DetalleMatchDto
    {
        [Required]
        public string Skill { get; set; }

        public int NivelColaborador { get; set; }

        public int NivelRequerido { get; set; }

        public double Puntaje { get; set; }
    }

    public class BrechaDetectadaDto
    {
        [Required]
        public string Skill { get; set; }

        public int NivelRequerido { get; set; }

        public double PromedioActual { get; set; }

        public int CantidadColaboradoresConSkill { get; set; }
    }
}
