using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class VacanteCreateDto
    {
        public string PerfilName { get; set; } = null!;
        public string IdArea { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public int Urgency { get; set; }
        public int State { get; set; }
        public string IdRol { get; set; } = null!;
        public List<string> Certifications { get; set; } = new();
        public List<string> Skills { get; set; } = new();
    }

    public class VacanteReadDto
    {
        public string Id { get; set; } = null!;
        public string PerfilName { get; set; } = null!;
        public string IdArea { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public int Urgency { get; set; }
        public int State { get; set; }
        public string IdRol { get; set; } = null!;
        public List<string> Certifications { get; set; } = new();
        public List<string> Skills { get; set; } = new();
    }
}
