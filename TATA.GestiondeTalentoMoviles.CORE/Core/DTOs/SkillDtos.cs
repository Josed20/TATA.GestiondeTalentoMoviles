using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class SkillCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
    }

    public class SkillUpdateDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
    }

    public class SkillReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
    }
}
