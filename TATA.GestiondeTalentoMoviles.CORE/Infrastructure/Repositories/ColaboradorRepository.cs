using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly IMongoCollection<Colaborador> _colaboradores;

        public ColaboradorRepository(IMongoDatabase database)
        {
            // ✅ Nombre EXACTO de la colección en MongoDB
            _colaboradores = database.GetCollection<Colaborador>("colaboradores");
        }

        public async Task<Colaborador> CreateAsync(Colaborador colaborador)
        {
            // ✅ No establecer Id, dejar que MongoDB lo genere automáticamente
            
            // Establecer fechas de auditoría
            colaborador.FechaRegistro = DateTime.UtcNow;
            colaborador.FechaActualizacion = DateTime.UtcNow;
            
            // Si el estado viene vacío, establecer "ACTIVO"
            if (string.IsNullOrWhiteSpace(colaborador.Estado))
            {
                colaborador.Estado = "ACTIVO";
            }
            
            await _colaboradores.InsertOneAsync(colaborador);
            return colaborador; // MongoDB habrá asignado el Id automáticamente
        }

        public async Task<IEnumerable<Colaborador>> GetAllAsync()
        {
            // ✅ Devolver TODOS los colaboradores sin filtrar por estado (incluye ACTIVO e INACTIVO)
            return await _colaboradores
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<Colaborador?> GetByIdAsync(string id)
        {
            // ✅ Buscar por Id SIN filtrar por estado (puede ser ACTIVO o INACTIVO)
            return await _colaboradores
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, Colaborador colaborador)
        {
            // ✅ Asegurar que el Id del documento coincida con el que se está actualizando
            colaborador.Id = id;
            
            // Actualizar fecha de modificación
            colaborador.FechaActualizacion = DateTime.UtcNow;

            var result = await _colaboradores.ReplaceOneAsync(
                c => c.Id == id,
                colaborador,
                new ReplaceOptions { IsUpsert = false }
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // ✅ Borrado lógico: marcar Estado como "INACTIVO"
            var update = Builders<Colaborador>.Update
                .Set(c => c.Estado, "INACTIVO")
                .Set(c => c.FechaActualizacion, DateTime.UtcNow);

            var result = await _colaboradores.UpdateOneAsync(
                c => c.Id == id,
                update
            );

            return result.ModifiedCount > 0;
        }
    }
}
