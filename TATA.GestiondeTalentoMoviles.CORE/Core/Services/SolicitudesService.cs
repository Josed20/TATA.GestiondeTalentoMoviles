using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudesRepository _repo;
        private readonly IColaboradorRepository _colaboradorRepo;

        public SolicitudService(
            ISolicitudesRepository repo,
            IColaboradorRepository colaboradorRepo)
        {
            _repo = repo;
            _colaboradorRepo = colaboradorRepo;
        }

        // ====================================
        // POST - Crear solicitud
        // ====================================
        public async Task<SolicitudReadDto> CreateAsync(SolicitudCreateDto dto, string creadoPorUsuarioId)
        {
            // Validar que el colaborador exista
            var colaboradorExiste = await _colaboradorRepo.GetByIdAsync(dto.ColaboradorId);
            if (colaboradorExiste == null)
            {
                throw new KeyNotFoundException($"El colaborador con ID '{dto.ColaboradorId}' no existe");
            }

            // Si el DTO trae CreadoPorUsuarioId, usar ese; si no, usar el parámetro
            var usuarioIdFinal = !string.IsNullOrWhiteSpace(dto.CreadoPorUsuarioId)
                ? dto.CreadoPorUsuarioId
                : creadoPorUsuarioId;

            // Si es solicitud de renovación, validar que la certificación anterior exista
            if (!string.IsNullOrWhiteSpace(dto.CertificacionIdAnterior))
            {
                // TODO: Validar con repositorio de certificaciones cuando esté disponible
                // Por ahora solo validamos que sea un ObjectId válido
                if (dto.CertificacionIdAnterior.Length != 24)
                {
                    throw new ArgumentException($"El ID de certificación anterior '{dto.CertificacionIdAnterior}' no es válido");
                }
            }

            // Validación específica para solicitudes de actualización de skills
            if (dto.TipoSolicitudGeneral == "ACTUALIZACION_SKILLS")
            {
                if (dto.CambiosSkillsPropuestos == null || dto.CambiosSkillsPropuestos.Count == 0)
                {
                    throw new ArgumentException("Para solicitudes de tipo ACTUALIZACION_SKILLS se requiere al menos un cambio de skill en 'cambiosSkillsPropuestos'");
                }
            }

            var entidad = MapCreateDtoToEntity(dto, usuarioIdFinal);

            var creada = await _repo.CreateAsync(entidad);

            return MapEntityToReadDto(creada);
        }

        // ====================================
        // GET ALL - Listar todas las solicitudes
        // ====================================
        public async Task<IEnumerable<SolicitudReadDto>> GetAllAsync()
        {
            var solicitudes = await _repo.GetAllAsync();
            return solicitudes.Select(MapEntityToReadDto).ToList();
        }

        // ====================================
        // GET BY ID - Obtener detalle por Id
        // ====================================
        public async Task<SolicitudReadDto?> GetByIdAsync(string id)
        {
            var solicitud = await _repo.GetByIdAsync(id);
            if (solicitud == null) return null;

            return MapEntityToReadDto(solicitud);
        }

        // ====================================
        // GET BY COLABORADOR - "Mis solicitudes"
        // ====================================
        public async Task<IEnumerable<SolicitudReadDto>> GetByColaboradorAsync(string colaboradorId)
        {
            // Validar que el colaborador exista
            var colaboradorExiste = await _colaboradorRepo.GetByIdAsync(colaboradorId);
            if (colaboradorExiste == null)
            {
                throw new KeyNotFoundException($"El colaborador con ID '{colaboradorId}' no existe");
            }

            var solicitudes = await _repo.GetByColaboradorAsync(colaboradorId);
            return solicitudes.Select(MapEntityToReadDto).ToList();
        }

        // ====================================
        // PUT - Actualizar estado de la solicitud
        // ====================================
        public async Task<SolicitudReadDto?> UpdateEstadoAsync(
            string id,
            SolicitudUpdateEstadoDto dto,
            string revisadoPorUsuarioId
        )
        {
            // Validar que la solicitud exista
            var solicitudExiste = await _repo.GetByIdAsync(id);
            if (solicitudExiste == null)
            {
                throw new KeyNotFoundException($"La solicitud con ID '{id}' no existe");
            }

            // Si el DTO trae RevisadoPorUsuarioId, usar ese; si no, usar el parámetro
            var usuarioIdFinal = !string.IsNullOrWhiteSpace(dto.RevisadoPorUsuarioId)
                ? dto.RevisadoPorUsuarioId
                : revisadoPorUsuarioId;

            var actualizada = await _repo.UpdateEstadoAsync(
                id,
                dto.EstadoSolicitud,
                dto.ObservacionAdmin,
                usuarioIdFinal
            );

            if (actualizada == null) return null;

            // TODO: Aplicar cambios automáticamente cuando se apruebe una solicitud
            // Si actualizada.TipoSolicitudGeneral == "ACTUALIZACION_SKILLS" 
            // && dto.EstadoSolicitud == "APROBADA"
            // entonces aplicar los cambios en actualizada.CambiosSkillsPropuestos
            // al Colaborador correspondiente:
            //   - Actualizar niveles de skills existentes
            //   - Modificar estado crítico de skills
            //   - Agregar nuevos skills al colaborador
            //   - Usar _colaboradorRepo para persistir los cambios

            return MapEntityToReadDto(actualizada);
        }

        // ====================================
        // DELETE - Eliminar solicitud
        // ====================================
        public async Task<bool> DeleteAsync(string id)
        {
            // Validar que la solicitud exista
            var solicitudExiste = await _repo.GetByIdAsync(id);
            if (solicitudExiste == null)
            {
                throw new KeyNotFoundException($"La solicitud con ID '{id}' no existe");
            }

            return await _repo.DeleteAsync(id);
        }

        // ====================================
        // Mapeos privados DTO <-> Entidad
        // ====================================

        private static Solicitud MapCreateDtoToEntity(SolicitudCreateDto dto, string creadoPorUsuarioId)
        {
            var solicitud = new Solicitud
            {
                // Campos base
                TipoSolicitudGeneral = dto.TipoSolicitudGeneral,
                TipoSolicitud = dto.TipoSolicitud,
                ColaboradorId = dto.ColaboradorId,
                CertificacionIdAnterior = dto.CertificacionIdAnterior,

                // Workflow
                EstadoSolicitud = "PENDIENTE",
                CreadoPorUsuarioId = creadoPorUsuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            // Si es solicitud de certificación
            if (dto.TipoSolicitudGeneral == "CERTIFICACION" && dto.CertificacionPropuesta != null)
            {
                solicitud.CertificacionPropuesta = new CertificacionPropuestaSolicitud
                {
                    Nombre = dto.CertificacionPropuesta.Nombre,
                    Institucion = dto.CertificacionPropuesta.Institucion,
                    FechaObtencion = dto.CertificacionPropuesta.FechaObtencion,
                    FechaVencimiento = dto.CertificacionPropuesta.FechaVencimiento,
                    ArchivoPdfUrl = dto.CertificacionPropuesta.ArchivoPdfUrl
                };
            }

            // Si es solicitud de entrevista de desempeño
            if (dto.TipoSolicitudGeneral == "ENTREVISTA_DESEMPENO" && dto.DatosEntrevistaPropuesta != null)
            {
                // Si el DTO trae PropuestoPorUsuarioId, usar ese; si no, usar creadoPorUsuarioId
                var propuestoPorId = !string.IsNullOrWhiteSpace(dto.DatosEntrevistaPropuesta.PropuestoPorUsuarioId)
                    ? dto.DatosEntrevistaPropuesta.PropuestoPorUsuarioId
                    : creadoPorUsuarioId;

                solicitud.DatosEntrevistaPropuesta = new DatosEntrevistaPropuestaSolicitud
                {
                    Motivo = dto.DatosEntrevistaPropuesta.Motivo,
                    Periodo = dto.DatosEntrevistaPropuesta.Periodo,
                    FechaSugerida = dto.DatosEntrevistaPropuesta.FechaSugerida,
                    PropuestoPorUsuarioId = propuestoPorId
                };
            }
            // Si es solicitud de actualización de skills
            if (dto.TipoSolicitudGeneral == "ACTUALIZACION_SKILLS"
                && dto.CambiosSkillsPropuestos != null)
            {
                solicitud.CambiosSkillsPropuestos = dto.CambiosSkillsPropuestos
                    .Select(c => new CambioSkillPropuestaSolicitud
                    {
                        Nombre = c.Nombre,
                        Tipo = c.Tipo,
                        NivelActual = c.NivelActual,
                        NivelPropuesto = c.NivelPropuesto,
                        EsCriticoActual = c.EsCriticoActual,
                        EsCriticoPropuesto = c.EsCriticoPropuesto,
                        Motivo = c.Motivo
                    }).ToList();
            }


            return solicitud;
        }

        private static SolicitudReadDto MapEntityToReadDto(Solicitud s)
        {
            return new SolicitudReadDto
            {
                Id = s.Id!,
                TipoSolicitudGeneral = s.TipoSolicitudGeneral,
                TipoSolicitud = s.TipoSolicitud,
                ColaboradorId = s.ColaboradorId,
                CertificacionIdAnterior = s.CertificacionIdAnterior,
                CertificacionPropuesta = s.CertificacionPropuesta == null
                    ? null
                    : new CertificacionPropuestaReadDto
                    {
                        Nombre = s.CertificacionPropuesta.Nombre,
                        Institucion = s.CertificacionPropuesta.Institucion,
                        FechaObtencion = s.CertificacionPropuesta.FechaObtencion,
                        FechaVencimiento = s.CertificacionPropuesta.FechaVencimiento,
                        ArchivoPdfUrl = s.CertificacionPropuesta.ArchivoPdfUrl
                    },
                DatosEntrevistaPropuesta = s.DatosEntrevistaPropuesta == null
                    ? null
                    : new DatosEntrevistaPropuestaReadDto
                    {
                        Motivo = s.DatosEntrevistaPropuesta.Motivo,
                        Periodo = s.DatosEntrevistaPropuesta.Periodo,
                        FechaSugerida = s.DatosEntrevistaPropuesta.FechaSugerida,
                        PropuestoPorUsuarioId = s.DatosEntrevistaPropuesta.PropuestoPorUsuarioId
                    },
                CambiosSkillsPropuestos = s.CambiosSkillsPropuestos == null
                    ? null
                    : s.CambiosSkillsPropuestos.Select(c => new CambioSkillPropuestaReadDto
                    {
                        Nombre = c.Nombre,
                        Tipo = c.Tipo,
                        NivelActual = c.NivelActual,
                        NivelPropuesto = c.NivelPropuesto,
                        EsCriticoActual = c.EsCriticoActual,
                        EsCriticoPropuesto = c.EsCriticoPropuesto,
                        Motivo = c.Motivo
                    }).ToList(),
                EstadoSolicitud = s.EstadoSolicitud,
                ObservacionAdmin = s.ObservacionAdmin,
                CreadoPorUsuarioId = s.CreadoPorUsuarioId,
                RevisadoPorUsuarioId = s.RevisadoPorUsuarioId,
                FechaCreacion = s.FechaCreacion,
                FechaRevision = s.FechaRevision
            };
        }
    }
}
