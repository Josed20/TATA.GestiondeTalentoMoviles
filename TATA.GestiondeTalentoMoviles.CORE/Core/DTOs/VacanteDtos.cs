using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class SkillRequeridoVacanteDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int NivelDeseado { get; set; }
        public bool EsCritico { get; set; }
    }

    public class VacanteCreateDto
    {
        public string NombrePerfil { get; set; } = null!;

        public string Area { get; set; } = null!;

        public string RolLaboral { get; set; } = null!;

        public List<SkillRequeridoVacanteDto> SkillsRequeridos { get; set; } = new();

        public List<string> CertificacionesRequeridas { get; set; } = new();

        public DateTime FechaInicio { get; set; }

        public string Urgencia { get; set; } = "MEDIA";


        public string EstadoVacante { get; set; } = "ABIERTA";


        public string CreadaPorUsuarioId { get; set; } = null!;
    }

    public class VacanteReadDto
    {
        public string Id { get; set; } = null!;

        public string NombrePerfil { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolLaboral { get; set; } = null!;

        public List<SkillRequeridoVacanteDto> SkillsRequeridos { get; set; } = new();
        public List<string> CertificacionesRequeridas { get; set; } = new();

        public DateTime FechaInicio { get; set; }
        public string Urgencia { get; set; } = null!;
        public string EstadoVacante { get; set; } = null!;

        public string CreadaPorUsuarioId { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string UsuarioActualizacion { get; set; } = null!;
    }
}
