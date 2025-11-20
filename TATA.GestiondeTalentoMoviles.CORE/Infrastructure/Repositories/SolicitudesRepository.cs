using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class SolicitudesRepository : ISolicitudesRepository
    {
        private readonly IMongoCollection<Solicitud> _solicitudes;

        public SolicitudesRepository(IMongoDatabase database)
        {
            // Nombre de la colección en MongoDB
            _solicitudes = database.GetCollection<Solicitud>("solicitudes");
        }

        // ====================================
        // POST - Crear solicitud
        // ====================================
        public async Task<Solicitud> CreateAsync(Solicitud solicitud)
        {
            // Defaults básicos de auditoría / workflow
            if (string.IsNullOrWhiteSpace(solicitud.EstadoSolicitud))
            {
                solicitud.EstadoSolicitud = "PENDIENTE";
            }

            solicitud.FechaCreacion = DateTime.UtcNow;

            await _solicitudes.InsertOneAsync(solicitud);
            return solicitud;
        }

        // ====================================
        // GET ALL - Listar todas las solicitudes
        // (el filtrado por tipo/estado se puede hacer en el Service)
        // ====================================
        public async Task<IEnumerable<Solicitud>> GetAllAsync()
        {
            return await _solicitudes
                .Find(_ => true)
                .ToListAsync();
        }

        // ====================================
        // GET BY ID - Obtener una solicitud por su Id
        // ====================================
        public async Task<Solicitud?> GetByIdAsync(string id)
        {
            return await _solicitudes
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync();
        }

        // ====================================
        // GET BY COLABORADOR - Todas las solicitudes de un colaborador
        // (útil para vista "Mis solicitudes" del colaborador)
        // ====================================
        public async Task<IEnumerable<Solicitud>> GetByColaboradorAsync(string colaboradorId)
        {
            return await _solicitudes
                .Find(s => s.ColaboradorId == colaboradorId)
                .ToListAsync();
        }

        // ====================================
        // PUT - Actualizar estado de la solicitud
        // (APROBADA / RECHAZADA / PROGRAMADA, etc.)
        // ====================================
        public async Task<Solicitud?> UpdateEstadoAsync(
            string id,
            string estadoSolicitud,
            string? observacionAdmin,
            string? revisadoPorUsuarioId
        )
        {
            var update = Builders<Solicitud>.Update
                .Set(s => s.EstadoSolicitud, estadoSolicitud)
                .Set(s => s.ObservacionAdmin, observacionAdmin)
                .Set(s => s.FechaRevision, DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(revisadoPorUsuarioId))
            {
                update = update.Set(s => s.RevisadoPorUsuarioId, revisadoPorUsuarioId);
            }

            return await _solicitudes.FindOneAndUpdateAsync(
                s => s.Id == id,
                update,
                new FindOneAndUpdateOptions<Solicitud>
                {
                    ReturnDocument = ReturnDocument.After // devuelve la solicitud ya actualizada
                }
            );
        }

        // ====================================
        // DELETE - Eliminar solicitud
        // (borrado físico; si quieres lógico, se cambia aquí)
        // ====================================
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _solicitudes.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
