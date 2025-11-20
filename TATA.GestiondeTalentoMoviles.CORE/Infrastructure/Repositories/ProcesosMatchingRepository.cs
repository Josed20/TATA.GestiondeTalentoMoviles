using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class ProcesosMatchingRepository : IProcesosMatchingRepository
    {
        private readonly IMongoCollection<ProcesosMatching> _collection;

        public ProcesosMatchingRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<ProcesosMatching>("procesos_matching");
        }

        public async Task CreateAsync(ProcesosMatching proceso) =>
            await _collection.InsertOneAsync(proceso);

        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(p => p.Id == id);

        public async Task<IEnumerable<ProcesosMatching>> GetAllAsync() =>
            await _collection.Find(p => true).ToListAsync();

        public async Task<ProcesosMatching> GetByIdAsync(string id) =>
            await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task UpdateAsync(string id, ProcesosMatching procesoIn) =>
            await _collection.ReplaceOneAsync(p => p.Id == id, procesoIn);

        public async Task<IEnumerable<ProcesosMatching>> GetByVacanteIdAsync(string vacanteId) =>
            await _collection.Find(p => p.VacanteId == vacanteId).ToListAsync();

        public async Task<IEnumerable<ProcesosMatching>> GetByFechaCreacionAsync(System.DateTime fechaCreacion)
        {
            // Buscar por la misma fecha (ignorando hora) en UTC
            var start = fechaCreacion.Date.ToUniversalTime();
            var end = start.AddDays(1);
            var filter = Builders<ProcesosMatching>.Filter.Gte(p => p.FechaCreacion, start) &
                         Builders<ProcesosMatching>.Filter.Lt(p => p.FechaCreacion, end);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
