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
    }
}
