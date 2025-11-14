using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class NivelSkillCreateDto
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class NivelSkillUpdateDto
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class NivelSkillReadDto
    {
        public string Id { get; set; } = null!;
        public int Codigo { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
