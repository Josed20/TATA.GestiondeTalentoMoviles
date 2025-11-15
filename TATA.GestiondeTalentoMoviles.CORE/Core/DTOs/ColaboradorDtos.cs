using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    // ====================================
    // DTOs para Certificaciones
    // ====================================

    public class CertificacionCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string? ImagenUrl { get; set; }
        public DateTime? FechaObtencion { get; set; }
        // Estado NO se envía, se setea en backend
    }

    public class CertificacionReadDto
    {
        public string Nombre { get; set; } = null!;
        public string? ImagenUrl { get; set; }
        public DateTime? FechaObtencion { get; set; }
        public string Estado { get; set; } = null!;
    }

    // ====================================
    // DTO para Disponibilidad
    // ====================================

    public class DisponibilidadDto
    {
        public string Estado { get; set; } = "Disponible";
        public int Dias { get; set; }
    }

    // ====================================
    // DTOs para Colaborador
    // ====================================

    public class ColaboradorCreateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;

        // Array de ObjectIds de skills
        public List<string> Skills { get; set; } = new();

        // Código del nivel (referencia a nivelskills.codigo)
        public int? NivelCodigo { get; set; }

        // Array de certificaciones
        public List<CertificacionCreateDto> Certificaciones { get; set; } = new();

        // Objeto disponibilidad
        public DisponibilidadDto Disponibilidad { get; set; } = new();
    }

    public class ColaboradorUpdateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;

        // Array de ObjectIds de skills
        public List<string> Skills { get; set; } = new();

        // Código del nivel
        public int? NivelCodigo { get; set; }

        // Array de certificaciones
        public List<CertificacionCreateDto> Certificaciones { get; set; } = new();

        // Objeto disponibilidad
        public DisponibilidadDto Disponibilidad { get; set; } = new();
    }

    public class ColaboradorReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;

        // Array de ObjectIds de skills
        public List<string> Skills { get; set; } = new();

        // Código del nivel
        public int? NivelCodigo { get; set; }

        // Array de certificaciones con estado
        public List<CertificacionReadDto> Certificaciones { get; set; } = new();

        // Objeto disponibilidad
        public DisponibilidadDto Disponibilidad { get; set; } = new();
    }
}
