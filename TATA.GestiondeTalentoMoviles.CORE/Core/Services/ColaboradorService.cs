using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly IColaboradorRepository _repository;

        public ColaboradorService(IColaboradorRepository repository)
        {
            _repository = repository;
        }

        // ====================================
        // Métodos CRUD básicos
        // ====================================

        public async Task<ColaboradorReadDto> CreateAsync(ColaboradorCreateDto createDto)
        {
            // Mapear CreateDto -> Entidad
            var colaborador = MapCreateDtoToEntity(createDto);

            // Crear en repositorio
            var nuevoColaborador = await _repository.CreateAsync(colaborador);

            // Entidad -> ReadDto
            return MapEntityToReadDto(nuevoColaborador);
        }

        public async Task<IEnumerable<ColaboradorReadDto>> GetAllAsync()
        {
            var colaboradores = await _repository.GetAllAsync();
            return colaboradores.Select(MapEntityToReadDto).ToList();
        }

        public async Task<ColaboradorReadDto?> GetByIdAsync(string id)
        {
            var colaborador = await _repository.GetByIdAsync(id);
            if (colaborador == null) return null;

            return MapEntityToReadDto(colaborador);
        }

        public async Task<ColaboradorReadDto?> UpdateAsync(string id, ColaboradorUpdateDto updateDto)
        {
            // Verificar si el colaborador existe
            var colaboradorExistente = await _repository.GetByIdAsync(id);
            if (colaboradorExistente == null) return null;

            // Mapear UpdateDto -> Entidad (preservando FechaRegistro del existente)
            var colaboradorActualizado = MapUpdateDtoToEntity(colaboradorExistente, updateDto);

            // Actualizar en repositorio
            var actualizado = await _repository.UpdateAsync(id, colaboradorActualizado);
            
            if (!actualizado) return null;

            // Entidad -> ReadDto
            return MapEntityToReadDto(colaboradorActualizado);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // Verificar si el colaborador existe
            var colaboradorExistente = await _repository.GetByIdAsync(id);
            if (colaboradorExistente == null) return false;

            // Borrado lógico: marcar como INACTIVO
            return await _repository.DeleteAsync(id);
        }

        // ====================================
        // Métodos privados de mapeo
        // ====================================

        private static Colaborador MapCreateDtoToEntity(ColaboradorCreateDto dto)
        {
            return new Colaborador
            {
                // Id se deja null, MongoDB lo generará automáticamente
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Correo = dto.Correo,
                Area = dto.Area,
                RolLaboral = dto.RolLaboral,
                Estado = "ACTIVO", // Por defecto
                DisponibleParaMovilidad = dto.DisponibleParaMovilidad,
                Skills = dto.Skills.Select(s => new SkillColaborador
                {
                    Nombre = s.Nombre,
                    Tipo = s.Tipo,
                    Nivel = s.Nivel,
                    EsCritico = s.EsCritico
                }).ToList(),
                Certificaciones = dto.Certificaciones.Select(c => new CertificacionColaborador
                {
                    Nombre = c.Nombre,
                    Institucion = c.Institucion,
                    FechaObtencion = c.FechaObtencion,
                    FechaVencimiento = c.FechaVencimiento,
                    ArchivoPdfUrl = c.ArchivoPdfUrl,
                    Estado = "VIGENTE",
                    FechaRegistro = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                }).ToList()
                // FechaRegistro y FechaActualizacion a nivel colaborador se completan en el repositorio
            };
        }

        private static Colaborador MapUpdateDtoToEntity(Colaborador existente, ColaboradorUpdateDto dto)
        {
            return new Colaborador
            {
                Id = existente.Id, // Mantener el mismo ID del existente
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Correo = dto.Correo,
                Area = dto.Area,
                RolLaboral = dto.RolLaboral,
                DisponibleParaMovilidad = dto.DisponibleParaMovilidad,
                Estado = !string.IsNullOrWhiteSpace(dto.Estado) ? dto.Estado : existente.Estado,
                FechaRegistro = existente.FechaRegistro, // Preservar FechaRegistro original
                Skills = dto.Skills.Select(s => new SkillColaborador
                {
                    Nombre = s.Nombre,
                    Tipo = s.Tipo,
                    Nivel = s.Nivel,
                    EsCritico = s.EsCritico
                }).ToList(),
                Certificaciones = dto.Certificaciones.Select(c => new CertificacionColaborador
                {
                    Nombre = c.Nombre,
                    Institucion = c.Institucion,
                    FechaObtencion = c.FechaObtencion,
                    FechaVencimiento = c.FechaVencimiento,
                    ArchivoPdfUrl = c.ArchivoPdfUrl,
                    Estado = "VIGENTE",
                    FechaRegistro = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                }).ToList()
                // FechaActualizacion a nivel colaborador se actualiza en el repositorio antes del Replace
            };
        }

        private static ColaboradorReadDto MapEntityToReadDto(Colaborador c)
        {
            return new ColaboradorReadDto
            {
                Id = c.Id!,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Correo = c.Correo,
                Area = c.Area,
                RolLaboral = c.RolLaboral,
                Estado = c.Estado,
                DisponibleParaMovilidad = c.DisponibleParaMovilidad,
                Skills = c.Skills.Select(s => new SkillReadDto
                {
                    Nombre = s.Nombre,
                    Tipo = s.Tipo,
                    Nivel = s.Nivel,
                    EsCritico = s.EsCritico
                }).ToList(),
                Certificaciones = c.Certificaciones.Select(cert => new CertificacionReadDto
                {
                    CertificacionId = cert.CertificacionId,
                    Nombre = cert.Nombre,
                    Institucion = cert.Institucion,
                    FechaObtencion = cert.FechaObtencion,
                    FechaVencimiento = cert.FechaVencimiento,
                    ArchivoPdfUrl = cert.ArchivoPdfUrl,
                    Estado = cert.Estado,
                    FechaRegistro = cert.FechaRegistro,
                    FechaActualizacion = cert.FechaActualizacion,
                    ProximaEvaluacion = cert.ProximaEvaluacion
                }).ToList(),
                FechaRegistro = c.FechaRegistro,
                FechaActualizacion = c.FechaActualizacion
            };
        }
    }
}
