using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class VacanteRepository : IVacanteRepository
    {
        private readonly IMongoCollection<Vacante> _vacantes;

        public VacanteRepository(IMongoDatabase database)
        {
            // El nombre de la colección en la BD es "vacantes" (minúsculas)
            _vacantes = database.GetCollection<Vacante>("vacantes");
        }

        public async Task<Vacante> CreateAsync(Vacante vacante)
        {
            await _vacantes.InsertOneAsync(vacante);
            return vacante;
        }

        public async Task<IEnumerable<Vacante>> GetAllAsync()
        {
            return await _vacantes.Find(_ => true).ToListAsync();
        }

        public async Task<Vacante?> GetByIdAsync(string id)
        {
            return await _vacantes.Find(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Vacante?> UpdateAsync(string id, Vacante vacante)
        {
            var filter = Builders<Vacante>.Filter.Eq(v => v.Id, id);
            var result = await _vacantes.ReplaceOneAsync(filter, vacante);
            if (result.MatchedCount == 0) return null;
            // Return the updated document (assume vacante contains Id)
            return vacante;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _vacantes.DeleteOneAsync(v => v.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
