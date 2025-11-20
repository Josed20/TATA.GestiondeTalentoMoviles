using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class ProcesosMatchingService : IProcesosMatchingService
    {
        private readonly IProcesosMatchingRepository _repo;

        public ProcesosMatchingService(IProcesosMatchingRepository repo)
        {
            _repo = repo;
        }

        public async Task<ProcesosMatchingViewDto> CreateAsync(ProcesosMatchingCreateDto dto)
        {
            var entity = MapToEntity(dto);
            entity.FechaCreacion = DateTime.UtcNow;

            await _repo.CreateAsync(entity);
            return MapToViewDto(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var exists = await _repo.GetByIdAsync(id);
            if (exists == null)
                throw new InvalidOperationException("Proceso de matching no encontrado.");

            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProcesosMatchingViewDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToViewDto);
        }

        public async Task<ProcesosMatchingViewDto> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToViewDto(entity);
        }

        public async Task<ProcesosMatchingViewDto> UpdateAsync(string id, ProcesosMatchingUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Proceso de matching no encontrado.");

            // Actualizar campos (preservando FechaCreacion e Id)
            existing.VacanteId = dto.VacanteId;
            existing.EjecutadoPorUsuarioId = dto.EjecutadoPorUsuarioId;
            existing.FechaEjecucion = dto.FechaEjecucion;
            existing.UmbralMatch = dto.UmbralMatch;
            existing.Candidatos = dto.Candidatos?.Select(c => new Candidato
            {
                ColaboradorId = c.ColaboradorId,
                NombreColaborador = c.NombreColaborador,
                PorcentajeMatch = c.PorcentajeMatch,
                Disponibilidad = c.Disponibilidad,
                DetalleMatch = c.DetalleMatch?.Select(d => new DetalleMatch
                {
                    Skill = d.Skill,
                    NivelColaborador = d.NivelColaborador,
                    NivelRequerido = d.NivelRequerido,
                    Puntaje = d.Puntaje
                }).ToList() ?? new List<DetalleMatch>()
            }).ToList() ?? new List<Candidato>();

            existing.BrechasDetectadas = dto.BrechasDetectadas?.Select(b => new BrechaDetectada
            {
                Skill = b.Skill,
                NivelRequerido = b.NivelRequerido,
                PromedioActual = b.PromedioActual,
                CantidadColaboradoresConSkill = b.CantidadColaboradoresConSkill
            }).ToList() ?? new List<BrechaDetectada>();

            existing.ResultadoGlobal = dto.ResultadoGlobal;
            existing.MensajeSistema = dto.MensajeSistema;

            await _repo.UpdateAsync(id, existing);
            return MapToViewDto(existing);
        }

        private ProcesosMatching MapToEntity(ProcesosMatchingCreateDto dto)
        {
            return new ProcesosMatching
            {
                VacanteId = dto.VacanteId,
                EjecutadoPorUsuarioId = dto.EjecutadoPorUsuarioId,
                FechaEjecucion = dto.FechaEjecucion,
                UmbralMatch = dto.UmbralMatch,
                Candidatos = dto.Candidatos?.Select(c => new Candidato
                {
                    ColaboradorId = c.ColaboradorId,
                    NombreColaborador = c.NombreColaborador,
                    PorcentajeMatch = c.PorcentajeMatch,
                    Disponibilidad = c.Disponibilidad,
                    DetalleMatch = c.DetalleMatch?.Select(d => new DetalleMatch
                    {
                        Skill = d.Skill,
                        NivelColaborador = d.NivelColaborador,
                        NivelRequerido = d.NivelRequerido,
                        Puntaje = d.Puntaje
                    }).ToList() ?? new List<DetalleMatch>()
                }).ToList() ?? new List<Candidato>(),
                BrechasDetectadas = dto.BrechasDetectadas?.Select(b => new BrechaDetectada
                {
                    Skill = b.Skill,
                    NivelRequerido = b.NivelRequerido,
                    PromedioActual = b.PromedioActual,
                    CantidadColaboradoresConSkill = b.CantidadColaboradoresConSkill
                }).ToList() ?? new List<BrechaDetectada>(),
                ResultadoGlobal = dto.ResultadoGlobal,
                MensajeSistema = dto.MensajeSistema
            };
        }

        private ProcesosMatchingViewDto MapToViewDto(ProcesosMatching p)
        {
            return new ProcesosMatchingViewDto
            {
                Id = p.Id,
                VacanteId = p.VacanteId,
                EjecutadoPorUsuarioId = p.EjecutadoPorUsuarioId,
                FechaEjecucion = p.FechaEjecucion,
                UmbralMatch = p.UmbralMatch,
                Candidatos = p.Candidatos?.Select(c => new CandidatoDto
                {
                    ColaboradorId = c.ColaboradorId,
                    NombreColaborador = c.NombreColaborador,
                    PorcentajeMatch = c.PorcentajeMatch,
                    Disponibilidad = c.Disponibilidad,
                    DetalleMatch = c.DetalleMatch?.Select(d => new DetalleMatchDto
                    {
                        Skill = d.Skill,
                        NivelColaborador = d.NivelColaborador,
                        NivelRequerido = d.NivelRequerido,
                        Puntaje = d.Puntaje
                    }).ToList() ?? new List<DetalleMatchDto>()
                }).ToList() ?? new List<CandidatoDto>(),
                BrechasDetectadas = p.BrechasDetectadas?.Select(b => new BrechaDetectadaDto
                {
                    Skill = b.Skill,
                    NivelRequerido = b.NivelRequerido,
                    PromedioActual = b.PromedioActual,
                    CantidadColaboradoresConSkill = b.CantidadColaboradoresConSkill
                }).ToList() ?? new List<BrechaDetectadaDto>(),
                ResultadoGlobal = p.ResultadoGlobal,
                MensajeSistema = p.MensajeSistema,
                FechaCreacion = p.FechaCreacion
            };
        }
    }
}
