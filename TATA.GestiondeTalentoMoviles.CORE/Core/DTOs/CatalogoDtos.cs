using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class NivelSkillDto
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = null!;
    }

    public class CatalogoReadDto
    {
        public string Id { get; set; } = null!;
        public List<string> Areas { get; set; } = new();
        public List<string> RolesLaborales { get; set; } = new();
        public List<NivelSkillDto> NivelesSkill { get; set; } = new();
        public List<string> TiposSkill { get; set; } = new();

        // para secciones adicionales - guardamos el JSON como string
        public Dictionary<string, string>? AdditionalSections { get; set; }
    }

    public class CatalogoUpdateDto
    {
        public List<string>? Areas { get; set; }
        public List<string>? RolesLaborales { get; set; }
        public List<NivelSkillDto>? NivelesSkill { get; set; }
        public List<string>? TiposSkill { get; set; }

        // permitir secciones dinámicas como string JSON
        public Dictionary<string, string>? AdditionalSections { get; set; }
    }

    public class CatalogoFilterDto
    {
        // propiedad para solicitar solo una sección
        [Required]
        public string Seccion { get; set; } = null!; // "areas" | "rolesLaborales" | "nivelesSkill" | "tiposSkill"
    }

    public class CatalogoCreateSectionDto
    {
        [Required]
        public string NombreSeccion { get; set; } = null!;

        [Required]
        public object Data { get; set; } = null!; // debe ser un array (se validará en servicio)
    }
}