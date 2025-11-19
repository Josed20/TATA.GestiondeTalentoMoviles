using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository _repo;

        public AlertaService(IAlertaRepository repo)
        {
            _repo = repo;
        }

        public async Task<AlertaViewDto> CreateAsync(AlertaCreateDto dto)
        {
            var detalle = new AlertaDetalle
            {
                FechaProximaEvaluacion = dto.Detalle.FechaProximaEvaluacion,
                SkillsFaltantes = dto.Detalle.SkillsFaltantes?.Select(s => new SkillFaltante
                {
                    Nombre = s.Nombre,
                    NivelRequerido = s.NivelRequerido
                }).ToList(),
                Descripcion = dto.Detalle.Descripcion
            };

            var alerta = new Alerta
            {
                Tipo = dto.Tipo,
                Estado = dto.Estado,
                ColaboradorId = dto.ColaboradorId,
                VacanteId = dto.VacanteId,
                ProcesoMatchingId = dto.ProcesoMatchingId,
                Detalle = detalle,
                UsuarioResponsable = dto.UsuarioResponsable,
                Destinatarios = dto.Destinatarios.Select(d => new AlertaDestinatario
                {
                    UsuarioId = d.UsuarioId,
                    Tipo = d.Tipo
                }).ToList(),
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            await _repo.CreateAsync(alerta);

            return Map(alerta);
        }

        public async Task<IEnumerable<AlertaViewDto>> GetAllAsync()
        {
            var alertas = await _repo.GetAllAsync();
            return alertas.Select(Map);
        }

        public async Task<AlertaViewDto> GetByIdAsync(string id)
        {
            var alert = await _repo.GetByIdAsync(id);
            return alert == null ? null : Map(alert);
        }

        public async Task<AlertaViewDto> UpdateAsync(string id, AlertaUpdateDto dto)
        {
            var alerta = await _repo.GetByIdAsync(id);
            if (alerta == null)
                throw new InvalidOperationException("Alerta no encontrada.");

            alerta.Tipo = dto.Tipo;
            alerta.Estado = dto.Estado;
            alerta.VacanteId = dto.VacanteId;
            alerta.ProcesoMatchingId = dto.ProcesoMatchingId;
            alerta.UsuarioResponsable = dto.UsuarioResponsable;

            alerta.Detalle = new AlertaDetalle
            {
                FechaProximaEvaluacion = dto.Detalle.FechaProximaEvaluacion,
                SkillsFaltantes = dto.Detalle.SkillsFaltantes?.Select(s => new SkillFaltante
                {
                    Nombre = s.Nombre,
                    NivelRequerido = s.NivelRequerido
                }).ToList(),
                Descripcion = dto.Detalle.Descripcion
            };

            alerta.Destinatarios = dto.Destinatarios.Select(d => new AlertaDestinatario
            {
                UsuarioId = d.UsuarioId,
                Tipo = d.Tipo
            }).ToList();

            alerta.FechaActualizacion = DateTime.UtcNow;

            await _repo.UpdateAsync(id, alerta);

            return Map(alerta);
        }

        public async Task DeleteAsync(string id)
        {
            var alerta = await _repo.GetByIdAsync(id);
            if (alerta == null)
                throw new InvalidOperationException("Alerta no encontrada.");

            await _repo.DeleteAsync(id);
        }

        // 🔥 NOW IMPLEMENTING MISSING METHODS
        public async Task<IEnumerable<AlertaViewDto>> GetAlertasPorColaboradorAsync(string colaboradorId)
        {
            var alertas = await _repo.GetAllAsync();
            var filtradas = alertas.Where(a => a.ColaboradorId == colaboradorId);
            return filtradas.Select(Map);
        }

        public async Task<IEnumerable<AlertaViewDto>> GetAlertasPendientesAsync()
        {
            var alertas = await _repo.GetAllAsync();
            var filtradas = alertas.Where(a => a.Estado == "PENDIENTE");
            return filtradas.Select(Map);
        }

        private AlertaViewDto Map(Alerta a)
        {
            return new AlertaViewDto
            {
                Id = a.Id,
                Tipo = a.Tipo,
                Estado = a.Estado,
                ColaboradorId = a.ColaboradorId,
                VacanteId = a.VacanteId,
                ProcesoMatchingId = a.ProcesoMatchingId,
                UsuarioResponsable = a.UsuarioResponsable,
                FechaCreacion = a.FechaCreacion,
                FechaActualizacion = a.FechaActualizacion,

                Detalle = new AlertaDetalleDto
                {
                    FechaProximaEvaluacion = a.Detalle?.FechaProximaEvaluacion,
                    SkillsFaltantes = a.Detalle?.SkillsFaltantes?.Select(s => new SkillFaltanteDto
                    {
                        Nombre = s.Nombre,
                        NivelRequerido = s.NivelRequerido
                    }).ToList(),
                    Descripcion = a.Detalle?.Descripcion
                },

                Destinatarios = a.Destinatarios?.Select(d => new AlertaDestinatarioDto
                {
                    UsuarioId = d.UsuarioId,
                    Tipo = d.Tipo
                }).ToList()
            };
        }
    }
}

