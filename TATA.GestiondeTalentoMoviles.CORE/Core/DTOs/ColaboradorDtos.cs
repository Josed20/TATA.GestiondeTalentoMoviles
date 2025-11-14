using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    // DTO para cada skill dentro del colaborador (CREATE)
    public class ColaboradorSkillCreateDto
    {
        public string SkillId { get; set; } = null!;  // Id de la colección skills
        public int NivelId { get; set; }              // Id de la colección nivelskill (0..3)
        public List<string> Certificaciones { get; set; } = new();
    }

    // DTO para cada skill dentro del colaborador (READ)
    public class ColaboradorSkillReadDto
    {
        public string SkillId { get; set; } = null!;
        public int NivelId { get; set; }
        public List<string> Certificaciones { get; set; } = new();
    }

    // DTO para disponibilidad (lo usamos igual en create y read)
    public class DisponibilidadDto
    {
        public string Estado { get; set; } = "Disponible";
        public int Dias { get; set; }
    }

    public class ColaboradorCreateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;

        // Lista de skills que se asignarán al colaborador
        public List<ColaboradorSkillCreateDto> Skills { get; set; } = new();

        // Subobjeto de disponibilidad
        public DisponibilidadDto Disponibilidad { get; set; } = new();
    }

    // DTO para actualizar un colaborador existente
    public class ColaboradorUpdateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;

        // Lista de skills que se asignarán al colaborador
        public List<ColaboradorSkillCreateDto> Skills { get; set; } = new();

        // Subobjeto de disponibilidad
        public DisponibilidadDto Disponibilidad { get; set; } = new();
    }

    public class ColaboradorReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;

        public List<ColaboradorSkillReadDto> Skills { get; set; } = new();

        public DisponibilidadDto Disponibilidad { get; set; } = new();
    }
}
