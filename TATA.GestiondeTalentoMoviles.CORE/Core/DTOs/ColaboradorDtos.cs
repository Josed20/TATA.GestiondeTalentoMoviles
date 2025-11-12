using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class ColaboradorCreateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;
        public string SkillPrimario { get; set; } = null!;
        public string SkillSecundario { get; set; } = null!;
        public int NivelDominio { get; set; }
        public List<string> Certificaciones { get; set; } = new();
        public string Disponibilidad { get; set; } = "Disponible";
        public int? DiasDisponibilidad { get; set; }
    }
    public class ColaboradorReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;
        public string SkillPrimario { get; set; } = null!;
        public string SkillSecundario { get; set; } = null!;
        public int NivelDominio { get; set; }
        public List<string> Certificaciones { get; set; } = new();
        public string Disponibilidad { get; set; } = "Disponible";
        public int? DiasDisponibilidad { get; set; }
    }
}
