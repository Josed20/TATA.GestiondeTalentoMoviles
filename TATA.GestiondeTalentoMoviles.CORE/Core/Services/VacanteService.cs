using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class VacanteService : IVacanteService
    {
        private readonly IVacanteRepository _repository;

        public VacanteService(IVacanteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VacanteReadDto>> GetAllAsync()
        {
            var vacantes = await _repository.GetAllAsync();
            return vacantes.Select(MapToReadDto);
        }

        public async Task<VacanteReadDto?> GetByIdAsync(string id)
        {
            var vacante = await _repository.GetByIdAsync(id);
            return vacante == null ? null : MapToReadDto(vacante);
        }

        public async Task<VacanteReadDto> CreateAsync(VacanteCreateDto createDto)
        {
            var now = DateTime.UtcNow;

            var vacante = new Vacante
            {
                NombrePerfil = createDto.NombrePerfil,
                Area = createDto.Area,
                RolLaboral = createDto.RolLaboral,

                SkillsRequeridos = (createDto.SkillsRequeridos ?? new List<SkillRequeridoVacanteDto>())
                    .Select(s => new SkillRequeridoVacante
                    {
                        Nombre = s.Nombre,
                        Tipo = s.Tipo,
                        NivelDeseado = s.NivelDeseado,
                        EsCritico = s.EsCritico
                    }).ToList(),

                CertificacionesRequeridas = (createDto.CertificacionesRequeridas ?? new List<string>()).ToList(),

                FechaInicio = createDto.FechaInicio,
                Urgencia = createDto.Urgencia,
                EstadoVacante = createDto.EstadoVacante,

                CreadaPorUsuarioId = createDto.CreadaPorUsuarioId,
                FechaCreacion = now,
                FechaActualizacion = now,
                UsuarioActualizacion = createDto.CreadaPorUsuarioId
            };

            var nuevaVacante = await _repository.CreateAsync(vacante);
            return MapToReadDto(nuevaVacante);
        }

        public async Task<VacanteReadDto?> UpdateAsync(string id, VacanteCreateDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;

            // No tocamos CreadaPorUsuarioId ni FechaCreacion
            existing.NombrePerfil = updateDto.NombrePerfil;
            existing.Area = updateDto.Area;
            existing.RolLaboral = updateDto.RolLaboral;

            existing.SkillsRequeridos = (updateDto.SkillsRequeridos ?? new List<SkillRequeridoVacanteDto>())
                .Select(s => new SkillRequeridoVacante
                {
                    Nombre = s.Nombre,
                    Tipo = s.Tipo,
                    NivelDeseado = s.NivelDeseado,
                    EsCritico = s.EsCritico
                }).ToList();

            existing.CertificacionesRequeridas = (updateDto.CertificacionesRequeridas ?? new List<string>()).ToList();

            existing.FechaInicio = updateDto.FechaInicio;
            existing.Urgencia = updateDto.Urgencia;
            existing.EstadoVacante = updateDto.EstadoVacante;

            existing.FechaActualizacion = now;
            // Usamos el usuario que manda el front en el DTO como quien actualiza
            existing.UsuarioActualizacion = updateDto.CreadaPorUsuarioId;

            var updated = await _repository.UpdateAsync(id, existing);
            return updated == null ? null : MapToReadDto(updated);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static VacanteReadDto MapToReadDto(Vacante v)
        {
            return new VacanteReadDto
            {
                Id = v.Id!,

                NombrePerfil = v.NombrePerfil,
                Area = v.Area,
                RolLaboral = v.RolLaboral,

                SkillsRequeridos = (v.SkillsRequeridos ?? new List<SkillRequeridoVacante>())
                    .Select(s => new SkillRequeridoVacanteDto
                    {
                        Nombre = s.Nombre,
                        Tipo = s.Tipo,
                        NivelDeseado = s.NivelDeseado,
                        EsCritico = s.EsCritico
                    }).ToList(),

                CertificacionesRequeridas = (v.CertificacionesRequeridas ?? new List<string>()).ToList(),

                FechaInicio = v.FechaInicio,
                Urgencia = v.Urgencia,
                EstadoVacante = v.EstadoVacante,

                CreadaPorUsuarioId = v.CreadaPorUsuarioId,
                FechaCreacion = v.FechaCreacion,
                FechaActualizacion = v.FechaActualizacion,
                UsuarioActualizacion = v.UsuarioActualizacion
            };
        }
    }
}
