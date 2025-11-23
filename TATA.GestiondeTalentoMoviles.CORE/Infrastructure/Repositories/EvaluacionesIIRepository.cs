using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.INFRASTRUCTURE.Repositories
{
    public class EvaluacionesIIRepository : IEvaluacionesIIRepository
    {
        private readonly IMongoCollection<EvaluacionesII> _collection;

        public EvaluacionesIIRepository(IMongoDatabase database)
        {
            // Colección para las plantillas
            _collection = database.GetCollection<EvaluacionesII>("plantillas_evaluacion");
        }

        public async Task<IEnumerable<EvaluacionesII>> GetAllAsync()
            => await _collection.Find(_ => true).ToListAsync();

        public async Task<EvaluacionesII?> GetByIdAsync(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task AddAsync(EvaluacionesII plantilla)
            => await _collection.InsertOneAsync(plantilla);

        public async Task UpdateAsync(EvaluacionesII plantilla)
            => await _collection.ReplaceOneAsync(x => x.Id == plantilla.Id, plantilla);

        public async Task DeleteAsync(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
