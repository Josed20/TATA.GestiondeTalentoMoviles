using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

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
            await _colaboradores.InsertOneAsync(colaborador);
            return colaborador; // MongoDB habrá asignado el Id automáticamente
        }

        public async Task<IEnumerable<Colaborador>> GetAllAsync()
        {
            return await _colaboradores.Find(_ => true).ToListAsync();
        }

        public async Task<Colaborador?> GetByIdAsync(string id)
        {
            return await _colaboradores
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, Colaborador colaborador)
        {
            // ✅ Asegurar que el Id del documento coincida con el que se está actualizando
            colaborador.Id = id;

            var result = await _colaboradores.ReplaceOneAsync(
                c => c.Id == id,
                colaborador,
                new ReplaceOptions { IsUpsert = false }
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // ✅ Borrado lógico: marcar disponibilidad como "Inactivo"
            var update = Builders<Colaborador>.Update
                .Set(c => c.Disponibilidad.Estado, "Inactivo");

            var result = await _colaboradores.UpdateOneAsync(
                c => c.Id == id,
                update
            );

            return result.ModifiedCount > 0;
        }
    }
}
