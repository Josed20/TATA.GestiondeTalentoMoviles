using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly IMongoCollection<Area> _areas;

        public AreaRepository(IMongoDatabase database)
        {
            _areas = database.GetCollection<Area>("areas");
        }

        public async Task<Area> CreateAsync(Area area)
        {
            await _areas.InsertOneAsync(area);
            return area;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _areas.DeleteOneAsync(a => a.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Area>> GetAllAsync()
        {
            return await _areas.Find(_ => true).ToListAsync();
        }

        public async Task<Area?> GetByIdAsync(string id)
        {
            return await _areas.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Area?> GetByNombreAsync(string nombre)
        {
            return await _areas.Find(a => a.Nombre == nombre).FirstOrDefaultAsync();
        }

        public async Task<Area?> UpdateAsync(string id, Area area)
        {
            var res = await _areas.ReplaceOneAsync(a => a.Id == id, area);
            if (res.ModifiedCount > 0) return area;
            return null;
        }
    }
}