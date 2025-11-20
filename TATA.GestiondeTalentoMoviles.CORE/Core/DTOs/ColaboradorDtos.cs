using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    // ====================================
    // DTOs para Skills
    // ====================================

    public class SkillCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!; // "TECNICO" o "BLANDO"
        public int Nivel { get; set; } // 1-4
        public bool EsCritico { get; set; }
    }

    public class SkillReadDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int Nivel { get; set; }
        public bool EsCritico { get; set; }
    }

    // ====================================
    // DTOs para Certificaciones
    // ====================================

    public class CertificacionCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Institucion { get; set; } = null!;
        public DateTime? FechaObtencion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? ArchivoPdfUrl { get; set; }
        // Estado y fechas de auditoría NO se envían, se setean en backend
    }

    public class CertificacionReadDto
    {
        public string? CertificacionId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Institucion { get; set; } = null!;
        public DateTime? FechaObtencion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? ArchivoPdfUrl { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? ProximaEvaluacion { get; set; }
    }

    // ====================================
    // DTOs para Colaborador
    // ====================================

    public class ColaboradorCreateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolLaboral { get; set; } = null!;
        public bool DisponibleParaMovilidad { get; set; }

        // Skills embebidos
        public List<SkillCreateDto> Skills { get; set; } = new();

        // Certificaciones embebidas
        public List<CertificacionCreateDto> Certificaciones { get; set; } = new();

        // Estado se setea en backend como "ACTIVO", no incluirlo en el create
    }

    public class ColaboradorUpdateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolLaboral { get; set; } = null!;
        public bool DisponibleParaMovilidad { get; set; }

        // Opcional: permitir cambiar el estado desde UI (ej. desactivar)
        public string? Estado { get; set; }

        // Skills embebidos
        public List<SkillCreateDto> Skills { get; set; } = new();

        // Certificaciones embebidas
        public List<CertificacionCreateDto> Certificaciones { get; set; } = new();
    }

    public class ColaboradorReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolLaboral { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public bool DisponibleParaMovilidad { get; set; }

        // Skills embebidos
        public List<SkillReadDto> Skills { get; set; } = new();

        // Certificaciones embebidas
        public List<CertificacionReadDto> Certificaciones { get; set; } = new();

        // Auditoría
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
