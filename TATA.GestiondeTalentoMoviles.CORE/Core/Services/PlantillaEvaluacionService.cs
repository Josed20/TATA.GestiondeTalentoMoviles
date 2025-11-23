using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class PlantillaEvaluacionService : IPlantillaEvaluacionService
    {
        private readonly IMongoCollection<PlantillaEvaluacion> _collection;

        public PlantillaEvaluacionService(IMongoDatabase database)
        {
            _collection = database.GetCollection<PlantillaEvaluacion>("plantillas_evaluacion");
        }

        public async Task<IEnumerable<PlantillaEvaluacion>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<PlantillaEvaluacion?> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PlantillaEvaluacion> CreateAsync(PlantillaEvaluacion plantilla)
        {
            await _collection.InsertOneAsync(plantilla);
            return plantilla;
        }

        public async Task<bool> UpdateAsync(string id, PlantillaEvaluacion plantilla)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == id, plantilla);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
