using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.INFRASTRUCTURE.Repositories
{
    public class EvaluacionesRepository : IEvaluacionRepository
    {
        private readonly IMongoCollection<Evaluacion> _collection;

        public EvaluacionesRepository(IMongoDatabase database)
        {
            // Nombre real de tu colección en Mongo
            _collection = database.GetCollection<Evaluacion>("evaluaciones");
        }

        public async Task<IEnumerable<Evaluacion>> GetAllAsync()
            => await _collection.Find(_ => true).ToListAsync();

        public async Task<Evaluacion?> GetByIdAsync(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Evaluacion>> GetByColaboradorAsync(string colaboradorId)
            => await _collection.Find(x => x.ColaboradorId == colaboradorId).ToListAsync();

        public async Task AddAsync(Evaluacion evaluacion)
            => await _collection.InsertOneAsync(evaluacion);

        public async Task UpdateAsync(Evaluacion evaluacion)
            => await _collection.ReplaceOneAsync(x => x.Id == evaluacion.Id, evaluacion);

        public async Task DeleteAsync(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}

